using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Krypta.Models.database
{
    public class PortfolioDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public PortfolioDatabase(string path)
        {
            _database = new SQLiteAsyncConnection(path);
            _database.CreateTableAsync<PortfolioItem>().Wait();
        }

        public Task<List<PortfolioItem>> GetPortfolioAsync()
        {
            return _database.Table<PortfolioItem>().ToListAsync();
        }

        public Task<int> SaveCryptoAsync(PortfolioItem portfolioItem)
        {
            return portfolioItem.Id != 0 ? _database.UpdateAsync(portfolioItem) : _database.InsertAsync(portfolioItem);
        }

        public Task<int> DeletePortfolioAsync(PortfolioItem portfolioItem)
        {
            return _database.DeleteAsync(portfolioItem);
        }
        
        public Task<double> SumOfAllPortfolio()
        {
            return _database.ExecuteScalarAsync<double>("SELECT SUM(TotalWorth) FROM PortfolioItem");
        }
        
        public Task<int> CountOfAllPortfolio()
        {
            return _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM PortfolioItem");
        }
    }
}