using AutoMapper;
using MegaStonksService.Helpers;
using MegaStonksService.Models.CoinMarketCapApi;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MegaStonksService.Services.CoinMarketCapApi
{
    public interface ICMCApiService
    {
        Task<CryptoListingsModel> GetCryptoListingAsync();
        Task<CryptoQuoteModel> GetLatestCryptoQuote(string cryptoSymbol);
        Task<CryptoInfoModel> GetCryptoInfo(string cryptoSymbol);
        Task<CryptoHistoricalModel> GetTodaysChart(string cryptoSymbol, string currency);
        Task<CryptoHistoricalModel> GetHistoricalChart(string cryptoSymbol, string currency);
    }

    public class CMCApiService : ICMCApiService
    {
        private readonly DataContext _context;
        private readonly WebClient _client;
        private readonly CMCApiSettings _cmcApiSettings;
        private readonly IMapper _mapper;

        public CMCApiService(DataContext context, IMapper mapper, IOptions<CMCApiSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _cmcApiSettings = appSettings.Value;
            _client = new WebClient();
            _client.Headers.Add("User-Agent", "MegaStonksService-1.0");
            _client.Headers.Add("X-CMC_PRO_API_KEY", _cmcApiSettings.APIKey);
        }

        public async Task<CryptoListingsModel> GetCryptoListingAsync()
        {
            string listingsEndpoint;
            try
            {
                listingsEndpoint = _cmcApiSettings.Latest;
               
                var cryptoListResponse = await _client.DownloadStringTaskAsync(listingsEndpoint);
                //var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ" };
                var stocksFromJson = JsonConvert.DeserializeObject<CryptoListingsModel>(cryptoListResponse);

                if (stocksFromJson == null)
                {
                    throw new AppException("Crypto List from Coin Market Cap Api is Empty or Null");
                }

                return stocksFromJson;
            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Crypto List from CMP API");
            }
        }

        public async Task<CryptoQuoteModel> GetLatestCryptoQuote(string cryptoSymbol)
        {
            string quoteEndpoint;
            try
            {
                quoteEndpoint = _cmcApiSettings.Quote + $"?symbol={cryptoSymbol}&convert=USD,CAD";

                var quoteResponse = await _client.DownloadStringTaskAsync(quoteEndpoint);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CryptoQuoteResolver(cryptoSymbol)
                };
                var quoteFromJson = JsonConvert.DeserializeObject<CryptoQuoteModel>(quoteResponse, settings);

                if (quoteFromJson == null)
                {
                    throw new AppException("Crypto Asset from Coin Market Cap Api is Empty or Null");
                }
                return quoteFromJson;
            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Crypto Asset from CMP API");
            }
        }

        public async Task<CryptoInfoModel> GetCryptoInfo(string cryptoSymbol)
        {
            string infoEndpoint;
            try
            {
                infoEndpoint = _cmcApiSettings.Info + $"?symbol={cryptoSymbol.ToUpperInvariant()}";

                var quoteResponse = await _client.DownloadStringTaskAsync(infoEndpoint);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CryptoInfoResolver(cryptoSymbol)
                };
                var infoFromJson = JsonConvert.DeserializeObject<CryptoInfoModel>(quoteResponse, settings);

                if (infoFromJson == null)
                {
                    throw new AppException("Crypto Asset Info from Coin Market Cap Api is Empty or Null");
                }
                return infoFromJson;
            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Crypto Asset Info from CMP API");
            }
        }


        public async Task<CryptoHistoricalModel> GetTodaysChart(string cryptoSymbol, string currency)
        {
            
            try
            {
                string historicalEndpoint;
                var dateToday = DateTime.UtcNow;

                historicalEndpoint = _cmcApiSettings.Historical + $"?symbol={cryptoSymbol.ToUpperInvariant()}&time_period=hourly&time_start={dateToday.ToString("yyyy-MM-dd")}&time_end={dateToday.AddDays(1).ToString("yyyy-MM-dd")}&interval=hourly&convert={currency.ToUpperInvariant()}";

                var historicalResponse = await _client.DownloadStringTaskAsync(historicalEndpoint);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CryptoHistoricalResolver(currency.ToUpperInvariant())
                };
                var historyFrom = JsonConvert.DeserializeObject<CryptoHistoricalModel>(historicalResponse, settings);

                if (historyFrom == null)
                {
                    throw new AppException("Crypto Asset Info from Coin Market Cap Api is Empty or Null");
                }
                return historyFrom;
            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Crypto Asset Info from CMP API");
            }
        }

        public async Task<CryptoHistoricalModel> GetHistoricalChart(string cryptoSymbol, string currency)
        {

            try
            {
                string historicalEndpoint;
                var dateToday = DateTime.UtcNow;

                historicalEndpoint = _cmcApiSettings.Historical + $"?symbol={cryptoSymbol.ToUpperInvariant()}&time_period=daily&time_start={dateToday.AddDays(-29).ToString("yyyy-MM-dd")}&time_end={dateToday.AddDays(1).ToString("yyyy-MM-dd")}&interval=daily&convert={currency.ToUpperInvariant()}";

                var historicalResponse = await _client.DownloadStringTaskAsync(historicalEndpoint);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CryptoHistoricalResolver(currency.ToUpperInvariant())
                };
                var historyFrom = JsonConvert.DeserializeObject<CryptoHistoricalModel>(historicalResponse, settings);

                if (historyFrom == null)
                {
                    throw new AppException("Crypto Asset Info from Coin Market Cap Api is Empty or Null");
                }
                return historyFrom;
            }
            catch (Exception)
            {
                throw new AppException($"Could not retrieve Crypto Asset Info from CMP API");
            }
        }
    }
}
