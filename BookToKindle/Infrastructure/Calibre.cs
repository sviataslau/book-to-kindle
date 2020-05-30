using System.Diagnostics;
using System.IO;
using BookToKindle.Domain;

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
		public static Book Convert(Book source, BookFormat targetFormat, string arguments)
		{
			string? fileName = Path.GetFileNameWithoutExtension(source.FilePath);
			if (source.Format.Equals(targetFormat))
			{
				return source;
			}
			string? directoryName = Path.GetDirectoryName(source.FilePath);
			string outputPath = Path.Combine(directoryName ?? string.Empty, $"{fileName}{targetFormat}");
			Process process = Process.Start("ebook-convert", $"{source.FilePath} {outputPath} {arguments}");
			process.WaitForExit();
			return Book.FromFile(source.Title, outputPath);
		}
	}
}