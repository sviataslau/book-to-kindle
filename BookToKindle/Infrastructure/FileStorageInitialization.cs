using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BookToKindle.Infrastructure
{
	internal class FileStorageInitialization : IHostedService
	{
		private readonly string fileStorage;

		public FileStorageInitialization(IConfiguration configuration)
		{
			this.fileStorage = configuration["FileStorage"];
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			Directory.CreateDirectory(this.fileStorage);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}