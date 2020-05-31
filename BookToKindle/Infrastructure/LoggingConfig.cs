using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace BookToKindle.Infrastructure
{
	internal static class LoggingConfig
	{
		public static IWebHostBuilder UseLogging(this IWebHostBuilder hostBuilder)
		{
			hostBuilder.UseSerilog((hostBuilderContext, loggerConfiguration) =>
			{
				IConfiguration appConfiguration = hostBuilderContext.Configuration;
				loggerConfiguration.ReadFrom.Configuration(appConfiguration)
					.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
			});
			return hostBuilder;
		}
	}
}