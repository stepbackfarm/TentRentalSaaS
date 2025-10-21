namespace TentRentalSaaS.Api.DTOs
{
    public class QuoteRequestDto
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public AddressDto Address { get; set; } = new AddressDto();
    }
}
