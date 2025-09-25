using FinacialBackend.Model;

namespace FinacialBackend.Interfaces
{
    public interface IFMPServices
    {
        Task<Stock> FindStockBySymbolAsync(string symbol);
    }
}
  