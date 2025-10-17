namespace TentRentalSaaS.Api.DTOs
{
    public class QuoteRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AddressDto Address { get; set; } = new AddressDto();
    }
}
