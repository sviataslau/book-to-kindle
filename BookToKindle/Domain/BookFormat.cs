using System;
using System.IO;

namespace BookToKindle.Domain
{
	/// <summary>
	/// Book format
	/// </summary>
	internal sealed class BookFormat
	{
		private readonly string extension;
		public readonly string Name;

		/// <summary>
		/// Creates book format based off the file extension
		/// </summary>
		/// <param name="extension">File extension, including leading dot (e.g. .mobi)</param>
		private BookFormat(string extension)
		{
			this.extension = extension;
			this.Name = extension.Substring(1).ToUpper();
		}

		public static readonly BookFormat Mobi = new BookFormat(".mobi");
		public static readonly BookFormat Azw3 = new BookFormat(".azw3");

		public override string ToString() => this.extension;

		public static implicit operator string(BookFormat format) => format.extension;

		public static BookFormat OfBookFile(string filePath) => new BookFormat(Path.GetExtension(filePath));

		private bool Equals(BookFormat other)
		{
			return this.extension.Equals(other.extension, StringComparison.CurrentCultureIgnoreCase);
		}

		public override bool Equals(object? obj)
		{
			return ReferenceEquals(this, obj) || obj is BookFormat other && Equals(other);
		}

		public override int GetHashCode()
		{
			return this.extension.GetHashCode();
		}
	}
}