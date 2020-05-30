using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookToKindle.Domain;
using BookToKindle.Infrastructure;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace BookToKindle
{
	[UsedImplicitly]
	internal class Program
	{
		private static readonly IConfiguration Configuration = Infrastructure.Configuration.Get();
		private static readonly ITelegramBotClient Bot = new TelegramBotClient(Configuration["ApiKey"]);
		private static readonly string FileStorage = Configuration["FileStorage"];

		private static readonly IDictionary<string, IBookConverter> Converters =
			new Dictionary<string, IBookConverter>
			{
				{ BookFormat.Mobi.Name, new Mobi() },
				{ BookFormat.Azw3.Name, new Azw3() }
			};

		public static async Task Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += ExceptionHappened;
			Logging.Setup(Configuration);
			Directory.CreateDirectory(FileStorage);

			User me = await Bot.GetMeAsync();
			Log.Information($"I am user {me.Id} and my name is {me.FirstName}.");

			Bot.OnMessage += MessageReceivedAsync;
			Bot.OnCallbackQuery += CallbackReceivedAsync;
			Bot.OnReceiveError += OnErrorReceived;
			Bot.StartReceiving(new[] { UpdateType.CallbackQuery, UpdateType.Message });

			Thread.Sleep(-1);
		}

		private static void OnErrorReceived(object? sender, ReceiveErrorEventArgs e)
		{
			Log.Error(e.ApiRequestException, "Exception happened");
		}

		private static void ExceptionHappened(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Error(e.ExceptionObject as Exception, "Exception happened");
		}

		private static async void MessageReceivedAsync(object? sender, MessageEventArgs e)
		{
			Message message = e.Message;
			long chatId = message.Chat.Id;

			if (message.Type != MessageType.Document)
			{
				await Bot.SendTextMessageAsync(chatId,
					"Sorry, I can't work with that. Send me a book file (mobi, epub, fb2, txt), please.",
					ParseMode.Markdown,
					replyToMessageId: message.MessageId);
				return;
			}

			Document document = message.Document;
			var formatSelectionKeyboard = new InlineKeyboardMarkup(new[]
			{
				Converters.Keys.Select(converter => InlineKeyboardButton.WithCallbackData(converter, converter))
			});
			await Bot.SendTextMessageAsync(chatId,
				$"Received *{document.FileName}*. Which format would you like to convert to?",
				ParseMode.Markdown,
				replyMarkup: formatSelectionKeyboard,
				replyToMessageId: message.MessageId);
		}

		private static async void CallbackReceivedAsync(object? sender, CallbackQueryEventArgs e)
		{
			CallbackQuery callbackQuery = e.CallbackQuery;
			Message message = callbackQuery.Message.ReplyToMessage;
			if (message == null || message.Type != MessageType.Document)
			{
				Log.Information("Unexpected original message {@Message}", message);
				return;
			}
			await ProcessBookAsync(message.Document, message.Chat.Id, callbackQuery.Data);
		}

		private static async Task ProcessBookAsync(Document document, long chatId, string desiredFormat)
		{
			using var tempFiles = new TempFiles();
			string fileName = document.FileName;
			try
			{
				await Bot.SendTextMessageAsync(chatId, $"Wait a bit! My dwarfs are working on *{fileName}*", ParseMode.Markdown);
				Book originalBook = await DownloadOriginalBookAsync(document);
				tempFiles.Add(originalBook.FilePath);
				Book convertedBook = ConvertBook(desiredFormat, originalBook);
				tempFiles.Add(convertedBook.FilePath);

				if (convertedBook.Equals(originalBook))
				{
					await Bot.SendTextMessageAsync(chatId, $"Your book *{originalBook.Title}* is already good!",
						ParseMode.Markdown);
				}
				else
				{
					await using Stream output = convertedBook.Content();
					await Bot.SendDocumentAsync(chatId,
						new InputOnlineFile(output, $"{convertedBook.Title}{convertedBook.Format}"),
						$"Here you go! Your *{convertedBook.Title}* is ready. Enjoy.",
						ParseMode.Markdown);
				}
			}
			catch (Exception exception)
			{
				Log.Error(exception, $"Exception happened when converting {document.FileId}");
				await Bot.SendTextMessageAsync(chatId,
					$"Sorry, something went wrong with *{fileName}*. Try sending the book again.",
					ParseMode.Markdown);
			}
		}

		private static async Task<Book> DownloadOriginalBookAsync(Document document)
		{
			string fileName = document.FileName;
			string originalBookPath = Path.Combine(FileStorage,
				$"{Guid.NewGuid().ToString()}{Path.GetExtension(fileName)}");
			await using Stream input = System.IO.File.OpenWrite(originalBookPath);
			File documentFile = await Bot.GetFileAsync(document.FileId);
			await Bot.DownloadFileAsync(documentFile.FilePath, input);
			return Book.FromFile(Path.GetFileNameWithoutExtension(fileName), originalBookPath);
		}

		private static Book ConvertBook(string desiredFormat, Book originalBook)
		{
			IBookConverter? converter = CreateConverter(desiredFormat);
			if (converter == null)
				throw new NotSupportedException($"No converter for {desiredFormat}");

			return converter.Convert(originalBook);

			static IBookConverter? CreateConverter(string format)
			{
				Converters.TryGetValue(format, out IBookConverter? bookConverter);
				return bookConverter;
			}
		}
	}
}