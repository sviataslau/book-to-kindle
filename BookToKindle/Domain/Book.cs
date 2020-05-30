using System.IO;

namespace BookToKindle.Domain
{
	/// <summary>
	/// Book from the filesystem
	/// </summary>
	internal sealed class Book
	{
		public readonly string Title;
		public readonly BookFormat Format;
		public readonly string FilePath;

		/// <summary>
		/// Create the book based off the existing file
		/// </summary>
		/// <param name="title">Title, without extension</param>
		/// <param name="format">Book format</param>
		/// <param name="filePath">Filesystem path to the book</param>
		private Book(string title, BookFormat format, string filePath)
		{
			this.Title = title;
			this.Format = format;
			this.FilePath = filePath;
		}

		public Stream Content() => File.OpenRead(this.FilePath);

		/// <summary>
		/// Create the book based off the existing file
		/// </summary>
		/// <param name="title">Title, without extension</param>
		/// <param name="path">Filesystem path to the book</param>
		/// <returns>Book object</returns>
		public static Book FromFile(string title, string path) => new Book(title, BookFormat.OfBookFile(path), path);
	}
}