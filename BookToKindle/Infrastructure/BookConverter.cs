using System.Threading;
using System.Threading.Tasks;
using BookToKindle.Domain;

namespace BookToKindle.Infrastructure
{
	internal class Azw3 : IBookConverter
	{
		private static BookFormat format = default!;

		public BookFormat Format =>
			LazyInitializer.EnsureInitialized(ref format, () => new BookFormat(".azw3", "AZW3"));

		public Task<Book> ConvertAsync(Book source)
		{
			return Calibre.ConvertAsync(source, Format, string.Empty);
		}
	}

	internal sealed class Mobi : IBookConverter
	{
		private static BookFormat format = default!;

		public BookFormat Format =>
			LazyInitializer.EnsureInitialized(ref format, () => new BookFormat(".mobi", "MOBI"));

		public Task<Book> ConvertAsync(Book source)
		{
			return Calibre.ConvertAsync(source, Format, "--mobi-file-type old");
		}
	}

	internal sealed class Mobi8 : IBookConverter
	{
		private static BookFormat format = default!;

		public BookFormat Format =>
			LazyInitializer.EnsureInitialized(ref format, () => new BookFormat(".mobi", "MOBI8"));

		public Task<Book> ConvertAsync(Book source)
		{
			return Calibre.ConvertAsync(source, Format, "--mobi-file-type new");
		}
	}
}