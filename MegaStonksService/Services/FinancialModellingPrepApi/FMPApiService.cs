using MegaStonksService.Entities.Assets;
using MegaStonksService.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.FinancialModellingPrepApi;
using System.Threading;

namespace MegaStonksService.Services.FinancialModellingPrepApi
{
    public class FMPApiService : IFMPApiService
    {
        private readonly DataContext _context;
        private readonly HttpClient _client;
        private readonly FMPApiSettings _fmpApiSettings;
        private readonly IMapper _mapper;

        public FMPApiService(DataContext context, IMapper mapper, IOptions<FMPApiSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _client = new HttpClient();
            _fmpApiSettings = appSettings.Value;
        }

        public async Task<List<StockModel>> GetStocksListAsync(string country)
        {
            string stocksEndpoint;
            try
            {
                if(country == "US")
                {
                    stocksEndpoint = _fmpApiSettings.Stocks + "?country=" + country + "&exchange=" + "nyse,nasdaq,amex" +
                                  "&apikey=" + _fmpApiSettings.APIKey;
                }
                else if(country == "CA")
                {
                    stocksEndpoint = _fmpApiSettings.Stocks + "?country=" + country + "&exchange=" + "tsx" +
                                                      "&apikey=" + _fmpApiSettings.APIKey;
                }
                else
                {
                    throw new AppException("Country Not Valid or Not Supported Currently");
                }
                 
                var stocksResponse = await _client.GetAsync(stocksEndpoint);
                string response2String = await stocksResponse.Content.ReadAsStringAsync();
                var stocksFromJson = JsonConvert.DeserializeObject<List<StockModel>>(response2String);

                if(stocksFromJson == null)
                {
                    throw new AppException("Stocks List from Finacial Modelling Prep Api is Empty or Null");
                }

                return stocksFromJson;
            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Stocks List from FMP API");
            }

        }

        public async Task<CurrencyExchangeResponse> ConvertCurrency(decimal amount, string currencyPair)
        {
            try
            {
                var currencyExchangeResponse = new CurrencyExchangeResponse();

                //Have to HardCode the currencypair here because the annoying API does not return the rate for CAD/USD but 
                //it returns the rate for USD/CAD. Abysmal!!
                string forexEndpoint = _fmpApiSettings.Forex + "USDCAD" + "?" +
                                 "&apikey=" + _fmpApiSettings.APIKey;
                var response = await _client.GetAsync(forexEndpoint);
                string responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<List<ForexModel>>(responseString);
                decimal exchangeRate = decimal.Parse(jsonResponse.FirstOrDefault().Bid);

                if(currencyPair == "USDCAD")
                {
                     currencyExchangeResponse = new CurrencyExchangeResponse
                     {
                        Amount = amount,
                        Rate = exchangeRate,
                        CurrencyPair = currencyPair,
                        Result = amount * exchangeRate
                     };
                }
                else if (currencyPair == "CADUSD")
                {
                    currencyExchangeResponse = new CurrencyExchangeResponse
                    {
                        Amount = amount,
                        Rate = 1 / exchangeRate,
                        CurrencyPair = currencyPair,
                        Result = amount / exchangeRate
                    };
                }
                else
                {
                    throw new AppException("Currency Pair not Currently Supported");
                }




                return currencyExchangeResponse;
            }
            catch (Exception)
            {
                throw new AppException($"Convert Currency failed. Please Check currency Pair");
            }

        }

        public async Task<StockProfileModel> GetStockProfile(int stockId)
        {
            try
            {
                var stock = _context.Stocks.Find(stockId);
                string profileEndpoint = _fmpApiSettings.Profile + stock.Symbol + "?apikey=" + _fmpApiSettings.APIKey;
                var response1 = await _client.GetAsync(profileEndpoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var profile = JsonConvert.DeserializeObject<List<StockProfileModel>>(responseString1).FirstOrDefault();

                if (profile == null || profile.Symbol == null || profile.Symbol == "")
                {
                    throw new AppException("Get Profile failed. API Provider does not have any Information for this Stock");
                }

                return profile;

            }
            catch (Exception)
            {
                throw new AppException("Get Profile failed. Please Check stock ID");
            }

        }

        public async Task<StockProfileModel> GetStockProfile2(int stockId)
        {
            try
            {
                var stock = _context.Stocks.Find(stockId);
                string profileEndpoint = _fmpApiSettings.Profile2 + stock.Symbol + "?apikey=" + _fmpApiSettings.APIKey;
                var response1 = await _client.GetAsync(profileEndpoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var profile = JsonConvert.DeserializeObject<List<StockProfileModel>>(responseString1).FirstOrDefault();

                if (profile == null || profile.Symbol == null || profile.Symbol == "")
                {
                    throw new AppException("Get Profile failed. API Provider does not have any Information for this Stock");
                }

                return profile;

            }
            catch (Exception)
            {
                throw new AppException("Get Profile failed. Please Check stock ID");
            }

        }

        public async Task<StockQuoteModel> GetStockQuote(int stockId)
        {
            try
            {
                var stock = _context.Stocks.Find(stockId);
                string quoteEndpoint = _fmpApiSettings.Quote + stock.Symbol + "?apikey=" + _fmpApiSettings.APIKey;
                var response1 = await _client.GetAsync(quoteEndpoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var quote = JsonConvert.DeserializeObject<List<StockQuoteModel>>(responseString1);

                if (quote == null || quote.FirstOrDefault().Symbol == null || quote.FirstOrDefault().Symbol == "")
                {
                    throw new AppException("Get Quote failed. API Provider does not have any Information for this Stock");
                }

                return quote.FirstOrDefault();

            }
            catch (Exception)
            {
                throw new AppException("Get quote failed. Please Check stock ID");
            }

        }

        public async Task<decimal> GetStockPrice(int stockId)
        {
            try
            {
                var stock = _context.Stocks.Find(stockId);
                string priceEndpoint = _fmpApiSettings.Price + stock.Symbol + "?apikey=" + _fmpApiSettings.APIKey;
                var response1 = await _client.GetAsync(priceEndpoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var price = JsonConvert.DeserializeObject<List<StockPriceModel>>(responseString1);

                if (price.FirstOrDefault() == null || price.Count <= 0)
                {
                    throw new AppException("Get Price failed. API Provider does not have any Information for this Stock");
                }

                return price.FirstOrDefault().Price;

            }
            catch (Exception)
            {
                throw new AppException($"Get Price failed. Please Check stock ID");
            }

        }

        public async Task<bool> IsMarketOpen()
        {
            try
            {
               
                string hoursEndpoint = _fmpApiSettings.MarketHours + "?apikey=" + _fmpApiSettings.APIKey;
                var response1 = await _client.GetAsync(hoursEndpoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var marketHours = JsonConvert.DeserializeObject<List<MarketHoursModel>>(responseString1);

                return marketHours.FirstOrDefault().IsTheStockMarketOpen;

            }
            catch (Exception)
            {
                throw new AppException($"Get Market Hours Failed");
            }

        }

        public async Task<List<PriceChartModel>> GetTodaysChart(int stockId)
        {
            try
            {
                var stock = _context.Stocks.Find(stockId);
                var dateYesterday = DateTime.UtcNow.AddDays(-1);
                var dateNow = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime dateFrom = TimeZoneInfo.ConvertTimeFromUtc(dateYesterday, easternZone);
                DateTime dateToday = TimeZoneInfo.ConvertTimeFromUtc(dateNow, easternZone);

                //Default here is 15 minute Intervals
                string endPoint = $"{_fmpApiSettings.DailyPriceChart}15min/{stock.Symbol}?from={dateToday.ToString("yyyy-MM-dd")}&to={dateToday.ToString("yyyy-MM-dd")}&apikey={_fmpApiSettings.APIKey}";
                var response1 = await _client.GetAsync(endPoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var priceChart = JsonConvert.DeserializeObject<List<PriceChartModel>>(responseString1);

                return priceChart;

            }
            catch (Exception)
            {
                throw new AppException($"Get Today's Price Chart Failed");
            }

        }

        public async Task<HistoricalPriceChartModel> GetHistoricalChart(int stockId)
        {
            try
            {
                    var stock = _context.Stocks.Find(stockId);
                    var date5YearsAgo = DateTime.UtcNow.AddYears(-5);
                    var dateNow = DateTime.UtcNow;
                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime dateFrom = TimeZoneInfo.ConvertTimeFromUtc(date5YearsAgo, easternZone);
                    DateTime dateToday = TimeZoneInfo.ConvertTimeFromUtc(dateNow, easternZone);

                    string endPoint = $"{_fmpApiSettings.DailyPriceChartFull}{stock.Symbol}?serietype=line&from={dateFrom.ToString("yyyy-MM-dd")}&to={dateToday.ToString("yyyy-MM-dd")}&apikey={_fmpApiSettings.APIKey}";
                    var response1 = await _client.GetAsync(endPoint);
                    string responseString1 = await response1.Content.ReadAsStringAsync();
                    var priceChart = JsonConvert.DeserializeObject<HistoricalPriceChartModel>(responseString1);
                return priceChart;

            }
            catch (Exception)
            {
                throw new AppException($"Get 5 Year historical Chart Failed");
            }

        }

        public async Task<List<StockNewsModel>> GetNews()
        {
            try
            {
                //For now, we are only going to get news for big tech. Eventually, this method will take stock symbols as parameters and fetch news for them

                //string newsEndpoint = _fmpApiSettings.StockNews + "?tickers=AAPL,FB,GOOG,AMZN,TSLA&limit=50" + "&apikey=" + _fmpApiSettings.APIKey;
                string newsEndpoint = _fmpApiSettings.StockNews + "?limit=100" + "&apikey=" + _fmpApiSettings.APIKey;
                var response1 = await _client.GetAsync(newsEndpoint);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                var stockNews = JsonConvert.DeserializeObject<List<StockNewsModel>>(responseString1);

                return stockNews;

            }
            catch (Exception)
            {
                throw new AppException($"Could not get stock news");
            }

        }

    }

}
