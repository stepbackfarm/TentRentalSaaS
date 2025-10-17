namespace TentRentalSaaS.Api.DTOs
{
    public class QuoteResponseDto
    {
        public decimal RentalFee { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal TotalPrice { get; set; }
        public int RentalDays { get; set; }
    }
}
