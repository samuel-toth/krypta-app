using System;
using SQLite;

namespace Krypta.Models
{
    public class PortfolioHistoryRecord
    {
        [PrimaryKey] [AutoIncrement] public int Id { get; set; }
        
        [NotNull] public double Amount { get; set; }
        
        public DateTime Date { get; set; }
        [NotNull] public int PortfolioCryptoId { get; set; }
    }
}