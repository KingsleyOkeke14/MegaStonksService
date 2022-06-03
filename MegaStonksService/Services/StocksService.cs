using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.FinancialModellingPrepApi;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Services
{
    public interface IStocksService
    {
        List<Stock> ListAllStocksInSchema();
        Task<List<Stock>> UpdateStocksInSchema(bool addStockstoSchema);
        List<Stock> SearchStock(string stockToSearch);
        StockInfoResponse GetStockInfo(int stockId, Account account);
        StockHolding GetStockHolding(Account account, int stockId);
        List<StockHoldingResponse> GetAllStockHoldings(Account account);
        List<PriceChartResponse> GetTodaysChart(int stockId);
        List<PriceChartResponse> GetHistoricalChart(int stockId, string interval);
        List<StockNewsModel> GetNews();
    }

    public class StocksService : IStocksService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFMPApiService _fmpApiService;
        private readonly IOrderService _orderService;

        public StocksService(DataContext context, IMapper mapper, IFMPApiService fmpApiService, IOrderService orderService)
        {
            _context = context;
            _mapper = mapper;
            _fmpApiService = fmpApiService;
            _orderService = orderService;
        }

        public List<Stock> ListAllStocksInSchema()
        {
            try
            {
                //await UpdateStocksInSchema();
                var stocksInSchema = _context.Stocks.ToList();
                return stocksInSchema;

            }
            catch (Exception)
            {
                throw new AppException("FMPApi - ListAllStocksInSchema could not list stocks in Database");
            }

        }

        public async Task<List<Stock>> UpdateStocksInSchema(bool addStockstoSchema)
        {
            try
            {

                var requestUSSTocks = await _fmpApiService.GetStocksListAsync("US");
                var requestCanadaSTocks = await _fmpApiService.GetStocksListAsync("CA");
                var stocksListUS = requestUSSTocks;
                var stocksListCanada = requestCanadaSTocks;
                var FMPApiStocks = stocksListUS.Union(stocksListCanada).OrderBy(x => x.Symbol).ToList();



                //Compare Stocks to See if there are any discrepancies 

                var dbStocks = _context.Stocks.AsNoTracking().OrderBy(x => x.Symbol).ToList();


                var dbStocksToModel = _mapper.Map<List<StockModel>>(dbStocks);
                var fMPApiStocksToModel = _mapper.Map<List<StockModel>>(FMPApiStocks);
                var newStocks = new List<StockModel>();


                //Check if There is a discrepancy in both lists
                if (dbStocksToModel.Count != fMPApiStocksToModel.Count)
                {

                    newStocks = fMPApiStocksToModel.Except(dbStocksToModel).ToList();
                }

                var stockstoAdd = _mapper.Map<List<Stock>>(newStocks);

                if (addStockstoSchema)
                {
                    foreach (var stock in stockstoAdd)
                    {
                        stock.LastUpdated = DateTime.UtcNow;
                        _context.Stocks.Add(stock);
                    }

                    _context.SaveChanges();
                }


                return stockstoAdd;

            }
            catch (Exception)
            {
                throw new AppException("UpdateStocksListInSchema could not pull missing stocks or could not update database with stocks list");
            }

        }

        public List<Stock> SearchStock(string stockToSearch)
        {
            try
            {
                var matchingStocksSymbol = _context.Stocks.AsNoTracking().Where(x => x.Symbol.Equals(stockToSearch.Trim().ToUpperInvariant())).ToList();
                var matchingStocksName = _context.Stocks.AsNoTracking().Where(x => x.CompanyName.Contains(stockToSearch.Trim())).ToList();

                var searchResult = matchingStocksSymbol.Union(matchingStocksName).Distinct().ToList();

                foreach(var stock in searchResult)
                {
                    if (stock.Exchange != null && stock.Exchange.ToUpper() == "TSXV")
                    {
                        stock.ExchangeShortName = "TSXV";
                    }
                }

                return searchResult.Select(x => x).Distinct(new StockComparer()).ToList();


            }
            catch (Exception)
            {
                throw new AppException("Could not find matching stock in database");
            }

        }

        public StockHolding GetStockHolding(Account account, int stockId)
        {
            try
            {
                _orderService.UpdateStockAndCryptoHoldings(account, false);
                var stock = _context.StockHoldings.AsNoTracking().Where(x => x.Account.Id == account.Id && x.Stock.Id == stockId).FirstOrDefault();

                return stock;
            }
            catch (Exception)
            {
                throw new AppException("Error Getting Stock in User's Holdings");
            }

        }

        public List<StockHoldingResponse> GetAllStockHoldings(Account account)
        {
            try
            {
                _orderService.UpdateStockAndCryptoHoldings(account, false);
                var allStockHoldings = new List<StockHoldingResponse>();
                var stockHoldings = _context.StockHoldings.AsNoTracking().Where(x => x.Account.Id == account.Id).ToList();
                foreach (var holding in stockHoldings)
                {
                    var stockId = _context.StockHoldings.AsNoTracking().Where(x => x.Id == holding.Id).Select(x => x.Stock).FirstOrDefault();
                    var stock = _context.Stocks.Find(stockId.Id);
                    var stockQuote = _fmpApiService.GetStockQuote(stock.Id).Result;


                    var stockToAdd = _mapper.Map<StockHoldingResponse>(holding);
                    stockToAdd.StockId = stockId.Id;
                    stockToAdd.Symbol = stock.Symbol;
                    stockToAdd.Exchange = stock.ExchangeShortName;
                    stockToAdd.ChangesPercentage = stockQuote.ChangesPercentage;
                    if (stock.Exchange.ToUpper() == "TSXV")
                    {
                        stockToAdd.Exchange = "TSXV";
                    }

                    
                    allStockHoldings.Add(stockToAdd);
                }


                return allStockHoldings;
            }
            catch (Exception)
            {
                throw new AppException("Error Getting User's Holdings");
            }

        }

        public StockInfoResponse GetStockInfo(int stockId, Account account)
        {
            try
            {
                //Get Stock Quote
                var stockQuote = _fmpApiService.GetStockQuote(stockId).Result;
                var stockProfile = _fmpApiService.GetStockProfile(stockId).Result;

                var stockInfo = _mapper.Map<StockInfoResponse>(stockQuote);
                stockInfo.Description = stockProfile.Description;
                stockInfo.Currency = stockProfile.Currency;

                if (stockProfile.Exchange.ToUpper() == "TSXV")
                {
                    stockInfo.Exchange = "TSXV";
                }


                //Check If Stock Exists in User's WatchList
                var watchlist = _context.WatchLists.AsNoTracking().Where(x => x.Account.Id == account.Id && x.Stock.Id == stockId).SingleOrDefault();

                if (watchlist != null)
                {
                    stockInfo.IsInWatchList = true;
                }

                //Check If Stock Exists in User's Holdings
                var holding = _context.StockHoldings.AsNoTracking().Where(x => x.Account.Id == account.Id && x.Stock.Id == stockId).SingleOrDefault();

                if (holding != null)
                {
                    stockInfo.IsInPortfolio = true;
                }
                stockInfo.StockId = stockId;
                return stockInfo;

            }
            catch (Exception)
            {

                throw new AppException($"Could not retrieve Stocks Info");
            }

        }

        public List<PriceChartResponse> GetTodaysChart(int stockId)
        {
            try
            {
                var chart = _fmpApiService.GetTodaysChart(stockId).Result;
                var result = _mapper.Map<List<PriceChartResponse>>(chart);

                return result;

            }
            catch (Exception)
            {

                throw new AppException($"Could not retrieve Stock's Price Chart For Today");
            }

        }

        public List<PriceChartResponse> GetHistoricalChart(int stockId, string interval)
        {
            try
            {
                var result = new List<PriceChartResponse>();
                var chart = _fmpApiService.GetHistoricalChart(stockId).Result;
                var priceCharts = _mapper.Map<List<PriceChartResponse>>(chart.Historical);


                if (interval == "5D")
                {
                    result = priceCharts.GetRange(0, 5);
                }
                else if (interval == "1M")
                {
                    result = priceCharts.GetRange(0, 30);
                }
                else if (interval == "3M")
                {
                    result = priceCharts.GetRange(0, 90);
                }
                else if (interval == "1Y")
                {
                    result = priceCharts.GetRange(0, 365);
                }
                else if (interval == "5Y")
                {
                    result = priceCharts.GetRange(0, 1200);
                }
                else
                {
                    throw new AppException($"Could not retrieve Historical Price: Invalid Time Interval");
                }

                if (result.Count <= 0)
                {
                    throw new AppException($"Could not retrieve Historical Price Chart");
                }

                return result;

            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Historical Price Chart");
            }

        }

        public List<StockNewsModel> GetNews()
        {
            try
            {
                var news = _fmpApiService.GetNews().Result;
                return news;

            }
            catch (Exception)
            {

                throw new AppException($"Could not retrieve Stock News");
            }

        }
    }
}