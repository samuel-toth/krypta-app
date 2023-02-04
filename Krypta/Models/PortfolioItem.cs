using SQLite;

namespace Krypta.Models
{
    public class PortfolioItem
    {
        [PrimaryKey] [AutoIncrement] public int Id { get; set; }

        [NotNull] public string Name { get; set; }

        [NotNull] public string Symbol { get; set; }

        [NotNull] public double Amount { get; set; }

        public string ImageUrl { get; set; }
        public double LatestPrice { get; set; }
        public string CoinGeckoId { get; set; }
        public double TotalWorth { get; set; }
    }
}