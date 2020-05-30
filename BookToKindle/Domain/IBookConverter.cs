namespace BookToKindle.Domain
{
	internal interface IBookConverter
	{
		Book Convert(Book source);
	}
}