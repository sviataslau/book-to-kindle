using System.Threading.Tasks;
using BookToKindle.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BookToKindle
{
	[Route("api/[controller]")]
	public class UpdateController : Controller
	{
		private readonly Bot bot;

		public UpdateController(Bot bot)
		{
			this.bot = bot;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Update update)
		{
			switch (update.Type)
			{
				case UpdateType.Message:
				{
					Message message = update.Message;
					if (message.IsDocument())
					{
						await this.bot.HandleDocumentAsync(message);
					}
					else
					{
						await this.bot.HandleTextAsync(message);
					}
					break;
				}
				case UpdateType.CallbackQuery:
				{
					CallbackQuery callback = update.CallbackQuery;
					await this.bot.HandleCallbackAsync(callback);
					break;
				}
				default:
				{
					Log.Information("Can't process {@Update}", update);
					break;
				}
			}
			return Ok();
		}
	}
}