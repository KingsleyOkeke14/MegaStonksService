using MegaStonksService.Entities.Assets;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.FinancialModellingPrepApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaStonksService.Services.FinancialModellingPrepApi
{
    public interface IFMPApiService
    {
        Task<List<StockModel>> GetStocksListAsync(string country);
        Task<CurrencyExchangeResponse> ConvertCurrency(decimal amount, string currencyPair);
        Task<StockQuoteModel> GetStockQuote(int stockId);
        Task<StockProfileModel> GetStockProfile(int stockId);
        Task<StockProfileModel> GetStockProfile2(int stockId);
        Task<bool> IsMarketOpen();
        Task<decimal> GetStockPrice(int stockId);
        Task<List<PriceChartModel>> GetTodaysChart(int stockId);
        Task<HistoricalPriceChartModel> GetHistoricalChart(int stockId);
        Task<List<StockNewsModel>> GetNews();
    }
}