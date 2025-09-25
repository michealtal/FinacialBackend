using FinacialBackend.Dtos.Stocks;
using FinacialBackend.Helpers;
using FinacialBackend.Model;

namespace FinacialBackend.Interfaces
{
    public interface IStockRepository
    {
        Task <List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id); // the ? is for FirstOrDefault and it can be null
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock?> DeleteAsync(int id);
        Task<bool> StockExist(int id);
    }
}
