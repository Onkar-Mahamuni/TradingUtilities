namespace PortfolioApis.Models
{
    public class Trade
    {
        public DateTime Date { get; set; }
        public string Company { get; set; }
        public decimal Amount { get; set; }
        public string Exchange { get; set; }
        public string Segment { get; set; }
        public string ScripCode { get; set; }
        public string InstrumentType { get; set; }
        public decimal StrikePrice { get; set; }
        public DateTime Expiry { get; set; }
        public int TradeNum { get; set; }
        public TimeSpan TradeTime { get; set; }
        public string Side { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
