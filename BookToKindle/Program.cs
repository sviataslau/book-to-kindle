using BookToKindle.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace BookToKindle
{
	[UsedImplicitly]
	internal class Program
	{
		public static void Main(string[] args)
		{
			IWebHost host = new WebHostBuilder()
				.UseKestrel()
				.UseConfiguration(Configuration.Get())
				.UseStartup<Startup>()
				.UseLogging()
				.Build();
			host.Run();
		}
	}
}