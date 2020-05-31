using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookToKindle.Domain;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace BookToKindle.Infrastructure
{
	public sealed class Bot
	{
		private static readonly IReadOnlyCollection<IBookConverter> Converters = new IBookConverter[]
			{ new Mobi(), new Mobi8(), new Azw3() };

		private static readonly IDictionary<BookFormat, IBookConverter> SupportedFormats =
			Converters.ToDictionary(c => c.Format, c => c);

		private static readonly InlineKeyboardMarkup FormatSelectionKeyboard = new InlineKeyboardMarkup(new[]
		{
			SupportedFormats.Keys.Select(bookFormat =>
				InlineKeyboardButton.WithCallbackData(bookFormat.Name, bookFormat.ToString()))
		});

		private readonly ITelegramBotClient bot;
		private readonly string fileStorage;

		public Bot(ITelegramBotClient bot, string fileStorage)
		{
			this.bot = bot;
			this.fileStorage = fileStorage;
		}

		public async Task HandleTextAsync(Message message)
		{
			if (message.IsDocument())
			{
				throw new ArgumentException($"Message {message.MessageId} is a document, not a text");
			}

			await this.bot.SendTextMessageAsync(message.Chat.Id,
				"Sorry, I can't work with that. Send me a book file (mobi, epub, fb2, txt), please.",
				ParseMode.Markdown,
				replyToMessageId: message.MessageId);
		}

		public async Task HandleDocumentAsync(Message message)
		{
			if (!message.IsDocument())
			{
				throw new ArgumentException($"Message {message.MessageId} is not a document");
			}

			Document document = message.Document;
			await this.bot.SendTextMessageAsync(message.Chat.Id,
				$"Received *{document.FileName}*. Which format would you like to convert to?",
				ParseMode.Markdown,
				replyMarkup: FormatSelectionKeyboard,
				replyToMessageId: message.MessageId);
		}

		public async Task HandleCallbackAsync(CallbackQuery callback)
		{
			Message message = callback.Message.ReplyToMessage;
			if (message == null || !message.IsDocument())
			{
				Log.Information("Unexpected original message {@Message}", message);
				return;
			}
			
			BookFormat? desiredFormat = BookFormat.TryParse(callback.Data);
			if (desiredFormat == null)
			{
				Log.Error("Can't parse callback {@Callback}", callback);
				return;
			}
			await ProcessBookAsync(message.Document, message.Chat.Id, desiredFormat);
		}

		private async Task ProcessBookAsync(Document document, long chatId, BookFormat desiredFormat)
		{
			using var tempFiles = new TempFiles();
			string fileName = document.FileName;
			try
			{
				await this.bot.SendTextMessageAsync(chatId, $"Wait a bit. My dwarfs are working on *{fileName}*",
					ParseMode.Markdown);
				Book originalBook = await DownloadOriginalBookAsync(document);
				tempFiles.Add(originalBook.FilePath);
				Book convertedBook = await ConvertBookAsync(desiredFormat, originalBook);
				tempFiles.Add(convertedBook.FilePath);

				if (convertedBook.Equals(originalBook))
				{
					await this.bot.SendTextMessageAsync(chatId, $"Your book *{originalBook.Title}* is already good!",
						ParseMode.Markdown);
				}
				else
				{
					await using Stream output = convertedBook.Content();
					await this.bot.SendDocumentAsync(chatId,
						new InputOnlineFile(output, $"{convertedBook.Title}{convertedBook.Format}"),
						$"Here you go! Your *{convertedBook.Title}* is ready. Enjoy.",
						ParseMode.Markdown);
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, $"Exception happened when converting {document.FileId}");
				await this.bot.SendTextMessageAsync(chatId,
					$"Sorry, something went wrong with *{fileName}*. Try sending the book again.",
					ParseMode.Markdown);
			}
		}

		private async Task<Book> DownloadOriginalBookAsync(Document document)
		{
			string fileName = document.FileName;
			string originalBookPath = Path.Combine(this.fileStorage,
				$"{Guid.NewGuid().ToString()}{Path.GetExtension(fileName)}");
			await using Stream input = System.IO.File.OpenWrite(originalBookPath);
			File documentFile = await this.bot.GetFileAsync(document.FileId);
			await this.bot.DownloadFileAsync(documentFile.FilePath, input);
			return new Book(Path.GetFileNameWithoutExtension(fileName),
				BookFormat.FromExtension(Path.GetExtension(fileName)), originalBookPath);
		}

		private static Task<Book> ConvertBookAsync(BookFormat desiredFormat, Book originalBook)
		{
			IBookConverter? converter = CreateConverter();
			if (converter == null)
			{
				throw new NotSupportedException($"No converter for {desiredFormat}");
			}

			return converter.ConvertAsync(originalBook);

			IBookConverter? CreateConverter()
			{
				SupportedFormats.TryGetValue(desiredFormat, out IBookConverter? bookConverter);
				return bookConverter;
			}
		}
	}
}