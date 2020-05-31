using System.Threading.Tasks;

namespace BookToKindle.Domain
{
	internal interface IBookConverter
	{
		BookFormat Format { get; }
		Task<Book> ConvertAsync(Book source);
	}
}