using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookToKindle.Domain;
using RunProcessAsTask;
using Serilog;

namespace BookToKindle.Infrastructure
{
	/// <summary>
	/// Calibre wrapper
	/// </summary>
	internal static class Calibre
	{
		/// <summary>
		/// Converts book using Calibre
		/// </summary>
		/// <param name="source">Source book</param>
		/// <param name="targetFormat">Target format file extension (e.g. .mobi)</param>
		/// <param name="arguments">Additional Calibre command line arguments</param>
		/// <returns>Converted book</returns>
		public static async Task<Book> ConvertAsync(Book source, BookFormat targetFormat, string arguments)
		{
			using MeasuredOperation operation =
				new MeasuredOperation($"Converting {source.Title} to {targetFormat.Name}");
			string? fileName = Path.GetFileNameWithoutExtension(source.FilePath);
			if (source.Format.Equals(targetFormat))
			{
				return source;
			}
			string? directoryName = Path.GetDirectoryName(source.FilePath);
			string outputPath = Path.Combine(directoryName ?? string.Empty, $"{fileName}{targetFormat}");
			ProcessResults result =
				await ProcessEx.RunAsync("ebook-convert", $"{source.FilePath} {outputPath} {arguments}");
			Log.Information("{@Output}", result.StandardOutput);
			if (result.StandardError.Any())
			{
				Log.Error("{@Output}", result.StandardError);
			}
			return new Book(source.Title, targetFormat, outputPath);
		}
	}
}