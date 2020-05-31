using BookToKindle.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace BookToKindle
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		[UsedImplicitly]
		public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
		{
			if (environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson();
			services.AddHostedService<FileStorageInitialization>();
			services.AddSingleton(new Bot(new TelegramBotClient(this.configuration["ApiKey"]),
				this.configuration["FileStorage"]));
		}
	}
}