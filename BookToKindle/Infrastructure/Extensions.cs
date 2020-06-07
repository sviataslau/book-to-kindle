using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BookToKindle.Infrastructure
{
	internal static class Extensions
	{
		public static bool IsDocument(this Message message) => message.Type == MessageType.Document;

		public static bool IsCommand(this Message message) =>
			message.Type == MessageType.Text && message.Text.StartsWith('/');
	}
}