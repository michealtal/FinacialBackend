using FinacialBackend.Interfaces;
using FinacialBackend.Mapper;
using FinacialBackend.Model;
using Newtonsoft.Json;

namespace FinacialBackend.Service
{
    public class FMPService : IFMPServices
    {
        private HttpClient _httpClient;
        private IConfiguration _config;
        public FMPService(HttpClient httpClient, IConfiguration config)
        {
          _httpClient = httpClient;
            _config = config;
        }
        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            try
            {
                var url = $"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_config["FMPKey"]}";
                var result = await _httpClient.GetAsync(url);

                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"FMP API call failed: {result.StatusCode}");
                    return null;
                }

                var content = await result.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content) || content == "[]")
                {
                    Console.WriteLine($"No stock data found for symbol: {symbol}");
                    return null;
                }

                var tasks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                var stock = tasks.FirstOrDefault();

                return stock?.ToStockFromFMP();    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock: {ex.Message}");
                return null;
            }
        }
    }
}
