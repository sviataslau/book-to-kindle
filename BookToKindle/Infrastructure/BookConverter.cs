using BookToKindle.Domain;

namespace BookToKindle.Infrastructure
{
	internal class Azw3 : IBookConverter
	{
		public Book Convert(Book source)
		{
			return Calibre.Convert(source, BookFormat.Azw3, string.Empty);
		}
	}

	internal sealed class Mobi : IBookConverter
	{
		public Book Convert(Book source)
		{
			return Calibre.Convert(source, BookFormat.Mobi, "--mobi-file-type old");
		}
	}
}