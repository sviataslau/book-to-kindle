using System.Threading;
using Microsoft.Extensions.Configuration;

namespace BookToKindle.Infrastructure
{
	internal static class Configuration
	{
		private static IConfiguration configuration = default!;

		public static IConfiguration Get() => LazyInitializer.EnsureInitialized(ref configuration, () =>
			new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
		);
	}
}