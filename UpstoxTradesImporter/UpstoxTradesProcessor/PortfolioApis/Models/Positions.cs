namespace PortfolioApis.Models
{
    public class Positions
    {
        public string PositionType { get; set; }
        public string Company { get; set; }
        public decimal IndividualCAGR { get; set; }
        public decimal AvgBuyPrice { get; set; }
        public decimal AvgSellPrice { get; set; }
        public int HoldingPeriod { get; set; }
        public decimal AbsoluteProfit { get; set; }
        public decimal PercentageProfit { get; set; }
        public int Quantity { get; set; }
        public decimal ProfitPerShare { get; set; }
        public List<Trade> Trades { get; set; }
    }
}
