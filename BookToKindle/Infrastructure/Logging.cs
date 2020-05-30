using Microsoft.Extensions.Configuration;
using Serilog;

namespace BookToKindle.Infrastructure
{
	internal static class Logging
	{
		public static void Setup(IConfiguration configuration)
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.CreateLogger();
		}
	}
}