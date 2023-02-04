using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Krypta.Models.database
{
    public class HistoryDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public HistoryDatabase(string path)
        {
            _database = new SQLiteAsyncConnection(path);
            _database.CreateTableAsync<PortfolioHistoryRecord>().Wait();
        }

        public Task<List<PortfolioHistoryRecord>> GetHistoryRecordsForPortfolioCryptoAsync(int id)
        {
            return _database.Table<PortfolioHistoryRecord>().Where(i => i.PortfolioCryptoId == id).ToListAsync();
        }

        public Task<int> SaveHistoryRecordAsync(PortfolioHistoryRecord portfolioHistoryRecord)
        {
            return portfolioHistoryRecord.Id != 0 ? _database.UpdateAsync(portfolioHistoryRecord) : _database.InsertAsync(portfolioHistoryRecord);
        }
        
        public Task<int> DeleteAllHistoryRecordsForItemAsync(int id)
        {
            return _database.ExecuteAsync("DELETE FROM PortfolioHistoryRecord WHERE PortfolioCryptoId = ?", id);
        }
    }
}