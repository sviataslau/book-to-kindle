using System.Threading;
using Microsoft.Extensions.Configuration;

namespace BookToKindle.Infrastructure
{
	public static class Configuration
	{
		private static IConfiguration configuration = default!;

		public static IConfiguration Get() => LazyInitializer.EnsureInitialized(ref configuration, () =>
			new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
		);
	}
}