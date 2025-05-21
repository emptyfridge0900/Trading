namespace Trading.Common.Models
{
    public record TradeRecord
    {
        public int Id { get; set; }
        
        public DateTime Time { get; set; }
        public string? Side {  get; set; }
        public string? Ticker {  get; set; }
        public float Price {  get; set; }
        public int Quantity {  get; set; }
        public float Total => Price * Quantity;

        public string? UserId { get; set; }
    }
}
