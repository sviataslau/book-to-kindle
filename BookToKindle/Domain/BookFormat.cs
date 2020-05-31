using System;

namespace BookToKindle.Domain
{
	/// <summary>
	/// Book format
	/// </summary>
	public sealed class BookFormat
	{
		private readonly string extension;
		public readonly string Name;

		/// <summary>
		/// Creates book format based off the file extension
		/// </summary>
		/// <param name="extension">File extension, including leading dot (e.g. .mobi)</param>
		/// <param name="name">Format name</param>
		public BookFormat(string extension, string name)
		{
			this.extension = extension;
			this.Name = name ?? extension.Substring(1).ToUpper();
		}

		public override string ToString() => $"{this.Name}|{this.extension}";

		public static BookFormat FromExtension(string extension) =>
			new BookFormat(extension, extension.Substring(1).ToUpper());

		public static BookFormat? TryParse(string input)
		{
			string[] split = input.Split('|');
			if (split.Length != 2)
			{
				return null;
			}
			return new BookFormat(split[1], split[0]);
		}

		private bool Equals(BookFormat other)
		{
			return this.extension.Equals(other.extension, StringComparison.CurrentCultureIgnoreCase) &&
			       this.Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase);
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