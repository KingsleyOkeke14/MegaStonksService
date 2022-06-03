using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.CoinMarketCapApi;
using MegaStonksService.Services.CoinMarketCapApi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Services
{
    public interface ICryptosService
    {
        List<Crypto> ListAllCryptoInSchema();
        Task<List<Crypto>> UpdateCryptosInSchema(bool addCryptosToSchema);
        List<Crypto> SearchCrypto(string cryptoToSearch);
        Task<Crypto> AddCryptoToSchema(string cryptoSymbol);
        Task<CryptoModel> GetQuote(string cryptoSymbol);
        Task<CryptoResponseModel> GetCrypto(int cryptoToGet, Account account);
        List<CryptoHoldingResponse> GetAllCryptoHoldings(Account account);
        CryptoHolding GetCryptoHolding(Account account, int cryptoId);

        List<PriceChartResponse> GetTodaysChart(Account account, int cryptoId);

        List<PriceChartResponse> GetHistoricalChart(Account account, int cryptoId);
    }
    public class CryptosService : ICryptosService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ICMCApiService _cmcApiService;
        private readonly IOrderService _orderService;

        public CryptosService(DataContext context, IMapper mapper, ICMCApiService cmcApiService, IOrderService orderService)
        {
            _context = context;
            _mapper = mapper;
            _cmcApiService = cmcApiService;
            _orderService = orderService;
        }

        public List<Crypto> ListAllCryptoInSchema()
        {
            try
            {
                var cryptosInSchema = _context.Cryptos.OrderBy(x => x.CmcRank).ToList();
                return cryptosInSchema;

            }
            catch (Exception)
            {
                throw new AppException("ListAllCryptosInSchema could not list cryptos in Database");
            }

        }

        public async Task<List<Crypto>> UpdateCryptosInSchema(bool addCryptosToSchema)
        {
            try
            {

                var latestCryptos = await _cmcApiService.GetCryptoListingAsync();
                var top100Cryptos = latestCryptos.Data.OrderBy(x => x.Symbol).ToList();
                //Compare Cryptos to See if there are any discrepancies 

                var dbCrytpos = _context.Cryptos.AsNoTracking().OrderBy(x => x.Symbol).ToList();


                var dbCryptosToModel = _mapper.Map<List<CryptoModel>>(dbCrytpos);
                var newCryptos = new List<CryptoModel>();

                newCryptos = top100Cryptos.Except(dbCryptosToModel).ToList();


                var cryptoToAdd = _mapper.Map<List<Crypto>>(newCryptos);

                if (addCryptosToSchema)
                {
                    foreach (var crypto in cryptoToAdd)
                    {
                        crypto.LastUpdated = DateTime.UtcNow;
                        _context.Cryptos.Add(crypto);
                    }

                    _context.SaveChanges();
                }


                return cryptoToAdd;

            }
            catch (Exception)
            {
                throw new AppException("UpdateCryptosInSchema could not pull missing cryptos or could not update database with cryptos list");
            }

        }

        public List<Crypto> SearchCrypto(string cryptoToSearch)
        {
            try
            {
                var matchingCryptoSymbol = _context.Cryptos.AsNoTracking().Where(x => x.Symbol.Equals(cryptoToSearch.Trim().ToUpperInvariant())).ToList();
                var matchingCryptoName = _context.Cryptos.AsNoTracking().Where(x => x.Name.Contains(cryptoToSearch.Trim())).ToList();

                var searchResult = matchingCryptoSymbol.Union(matchingCryptoName).Distinct().ToList();

                return searchResult.Select(x => x).Distinct(new CryptoComparer()).ToList();
            }
            catch (Exception)
            {
                throw new AppException("Could not find matching crypto in database");
            }

        }

        public async Task<CryptoModel> GetQuote(string cryptoSymbol)
        {
            try
            {

                var latestQuote = await _cmcApiService.GetLatestCryptoQuote(cryptoSymbol);

                return latestQuote.Data.Symbol;

            }
            catch (Exception)
            {
                throw new AppException("GetQuote could not pull latest crypto quote or could not update database with latest quote");
            }

        }

        public async Task<CryptoResponseModel> GetCrypto(int cryptoToGet, Account account)
        {
            try
            {
                await UpdateCryptoInfo(cryptoToGet);
                await UpdateCryptoQuotes(cryptoToGet);

                

                var crypto = _context.Cryptos.AsNoTracking().Where(x => x.Id == cryptoToGet).FirstOrDefault();
                var cryptoInfo = _context.CryptoInfo.Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();
                var cryptoQuoteUSD = _context.CryptoQuoteUSD.Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();
                var cryptoQuoteCAD = _context.CryptoQuoteCAD.Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();

                var cryptoResponse = new CryptoResponseModel()
                { 
                    Crypto = crypto,
                    Info = _mapper.Map<CryptoInfoResponse>(cryptoInfo),
                    USDQuote = _mapper.Map<CryptoQuoteResponse>(cryptoQuoteUSD),
                    CADQuote = _mapper.Map<CryptoQuoteResponse>(cryptoQuoteCAD),
                   
                };

                //Check If Crypto Exists in User's WatchList
                var watchlist = _context.CryptoWatchLists.AsNoTracking().Where(x => x.Account.Id == account.Id && x.Crypto.Id == cryptoToGet).SingleOrDefault();

                if (watchlist != null)
                {
                    cryptoResponse.IsInWatchlist = true;
                }

                //Check If Crypto Exists in User's Holdings
                var holding = _context.CryptoHoldings.AsNoTracking().Where(x => x.Account.Id == account.Id && x.Crypto.Id == cryptoToGet).SingleOrDefault();

                if (holding != null)
                {
                    cryptoResponse.IsInPortfolio = true;
                }

                return cryptoResponse;
            }
            catch (Exception)
            {
                throw new AppException("GetCrypto could not get crypto from Database");
            }

        }

        public async Task<Crypto> AddCryptoToSchema(string cryptoSymbol)
        {
            try
            {


                var latestQuote = await _cmcApiService.GetLatestCryptoQuote(cryptoSymbol.Trim().ToUpperInvariant());

                var newCrypto = new CryptoModel();

                newCrypto = latestQuote.Data.Symbol;


                var cryptoToAdd = _mapper.Map<Crypto>(newCrypto);

                bool isCryptoInSchema = _context.Cryptos.Where(x => x.Symbol == cryptoSymbol).Any();

                if (!isCryptoInSchema)
                {

                    cryptoToAdd.LastUpdated = DateTime.UtcNow;
                    _context.Cryptos.Add(cryptoToAdd);

                    _context.SaveChanges();
                }
                else
                {
                    throw new AppException("Error: Asset Already Exists in the Crypto Table");
                }


                return cryptoToAdd;

            }
            catch (Exception e)
            {
                throw new AppException("Add Crypto to Schema Could not add crypto to Schema" + e.Message);
            }

        }


        private async Task UpdateCryptoInfo(int cryptoToUpdate)
        {
            try
            {
                var crypto = _context.Cryptos.Find(cryptoToUpdate);
                var cryptoInfo = _context.CryptoInfo.Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();
                var newCryptoInfo = new CryptoInfoModel();
                bool needsUpdate = false;
                var timeNow = DateTime.UtcNow;

                if ((cryptoInfo == null) || cryptoInfo.LastUpdated <= timeNow.AddDays(-1))
                {
                    needsUpdate = true;
                    newCryptoInfo = await _cmcApiService.GetCryptoInfo(crypto.Symbol);
                }

                if (cryptoInfo == null && needsUpdate)
                {
                    var cryptoInfoToAdd = new CryptoInfo()
                    {
                        Crypto = crypto,
                        Category = newCryptoInfo.Data.Symbol.Category,
                        Description = newCryptoInfo.Data.Symbol.Description,
                        Logo = newCryptoInfo.Data.Symbol.Logo,
                        Website = newCryptoInfo.Data.Symbol.Urls.Website.Count == 0 ? "" : newCryptoInfo.Data.Symbol.Urls.Website.FirstOrDefault(),
                        Twitter = newCryptoInfo.Data.Symbol.Urls.Twitter.Count == 0 ? "" : newCryptoInfo.Data.Symbol.Urls.Twitter.FirstOrDefault().ToString(),
                        Reddit = newCryptoInfo.Data.Symbol.Urls.Reddit.Count == 0 ? "" : newCryptoInfo.Data.Symbol.Urls.Reddit.FirstOrDefault().ToString(),
                        LastUpdated = timeNow
                    };
                    _context.CryptoInfo.Add(cryptoInfoToAdd);
                    _context.SaveChanges();
                }
                else if (needsUpdate)
                {
                    cryptoInfo.Category = newCryptoInfo.Data.Symbol.Category;
                    cryptoInfo.Description = newCryptoInfo.Data.Symbol.Description;
                    cryptoInfo.Logo = newCryptoInfo.Data.Symbol.Logo;
                    cryptoInfo.Website = newCryptoInfo.Data.Symbol.Urls.Website.Count == 0 ? "" : newCryptoInfo.Data.Symbol.Urls.Website.FirstOrDefault();
                    cryptoInfo.Twitter = newCryptoInfo.Data.Symbol.Urls.Twitter.Count == 0 ? "" : newCryptoInfo.Data.Symbol.Urls.Twitter.FirstOrDefault().ToString();
                    cryptoInfo.Reddit = newCryptoInfo.Data.Symbol.Urls.Reddit.Count == 0 ? "" : newCryptoInfo.Data.Symbol.Urls.Reddit.FirstOrDefault().ToString();
                    cryptoInfo.LastUpdated = timeNow;

                    _context.CryptoInfo.Update(cryptoInfo);
                    await _context.SaveChangesAsync();
                }


            }
            catch (Exception)
            {
                throw new AppException("Could not Update CryptoInfo Table");
            }
        }

        private async Task UpdateCryptoQuotes(int cryptoToUpdate)
        {
            try
            {
                var crypto = _context.Cryptos.Find(cryptoToUpdate);
                var cryptoQuoteUSD = _context.CryptoQuoteUSD.Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();
                var cryptoQuoteCAD = _context.CryptoQuoteCAD.Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();
                var timeNow = DateTime.UtcNow;
                bool needsUpdate = false;
                var newQuote = new CryptoQuoteModel();

                if ((cryptoQuoteUSD == null && cryptoQuoteCAD == null) || cryptoQuoteUSD.LastUpdated <= timeNow.AddMinutes(-10))
                {
                    needsUpdate = true;
                    newQuote = await _cmcApiService.GetLatestCryptoQuote(crypto.Symbol);
                }



                if (cryptoQuoteUSD == null && cryptoQuoteCAD == null && needsUpdate)
                {
                    var usdQuote = new CryptoQuoteUSD()
                    {
                        Crypto = crypto,
                        Price = newQuote.Data.Symbol.Quote.USD.Price,
                        Volume24h = newQuote.Data.Symbol.Quote.USD.Volume24h ?? 0.0,
                        PercentChange1h = newQuote.Data.Symbol.Quote.USD.PercentChange1h ?? 0.0M,
                        PercentChange24h = newQuote.Data.Symbol.Quote.USD.PercentChange24h ?? 0.0M,
                        PercentChange7d = newQuote.Data.Symbol.Quote.USD.PercentChange7d ?? 0.0M,
                        PercentChange30d = newQuote.Data.Symbol.Quote.USD.PercentChange30d ?? 0.0M,
                        PercentChange60d = newQuote.Data.Symbol.Quote.USD.PercentChange60d ?? 0.0M,
                        PercentChange90d = newQuote.Data.Symbol.Quote.USD.PercentChange90d ?? 0.0M,
                        MarketCap = newQuote.Data.Symbol.Quote.USD.MarketCap ?? 0.0,
                        LastUpdated = timeNow
                    };

                    var cadQuote = new CryptoQuoteCAD()
                    {
                        Crypto = crypto,
                        Price = newQuote.Data.Symbol.Quote.CAD.Price,
                        Volume24h = newQuote.Data.Symbol.Quote.CAD.Volume24h ?? 0.0,
                        PercentChange1h = newQuote.Data.Symbol.Quote.CAD.PercentChange1h ?? 0.0M,
                        PercentChange24h = newQuote.Data.Symbol.Quote.CAD.PercentChange24h ?? 0.0M,
                        PercentChange7d = newQuote.Data.Symbol.Quote.CAD.PercentChange7d ?? 0.0M,
                        PercentChange30d = newQuote.Data.Symbol.Quote.CAD.PercentChange30d ?? 0.0M,
                        PercentChange60d = newQuote.Data.Symbol.Quote.CAD.PercentChange60d ?? 0.0M,
                        PercentChange90d = newQuote.Data.Symbol.Quote.CAD.PercentChange90d ?? 0.0M,
                        MarketCap = newQuote.Data.Symbol.Quote.CAD.MarketCap ?? 0.0,
                        LastUpdated = timeNow
                    };

                    _context.CryptoQuoteUSD.Add(usdQuote);
                    _context.CryptoQuoteCAD.Add(cadQuote);
                }
                else if (needsUpdate)
                {
                    //Update USD Quote
                    cryptoQuoteUSD.Price = newQuote.Data.Symbol.Quote.USD.Price;
                    cryptoQuoteUSD.Volume24h = newQuote.Data.Symbol.Quote.USD.Volume24h ?? 0.0;
                    cryptoQuoteUSD.PercentChange1h = newQuote.Data.Symbol.Quote.USD.PercentChange1h ?? 0.0M;
                    cryptoQuoteUSD.PercentChange24h = newQuote.Data.Symbol.Quote.USD.PercentChange24h ?? 0.0M;
                    cryptoQuoteUSD.PercentChange7d = newQuote.Data.Symbol.Quote.USD.PercentChange7d ?? 0.0M;
                    cryptoQuoteUSD.PercentChange30d = newQuote.Data.Symbol.Quote.USD.PercentChange30d ?? 0.0M;
                    cryptoQuoteUSD.PercentChange60d = newQuote.Data.Symbol.Quote.USD.PercentChange60d ?? 0.0M;
                    cryptoQuoteUSD.PercentChange90d = newQuote.Data.Symbol.Quote.USD.PercentChange90d ?? 0.0M;
                    cryptoQuoteUSD.MarketCap = newQuote.Data.Symbol.Quote.USD.MarketCap ?? 0.0;
                    cryptoQuoteUSD.LastUpdated = timeNow;

                    //Update CAD Quote
                    cryptoQuoteCAD.Price = newQuote.Data.Symbol.Quote.CAD.Price;
                    cryptoQuoteCAD.Volume24h = newQuote.Data.Symbol.Quote.CAD.Volume24h ?? 0.0;
                    cryptoQuoteCAD.PercentChange1h = newQuote.Data.Symbol.Quote.CAD.PercentChange1h ?? 0.0M;
                    cryptoQuoteCAD.PercentChange24h = newQuote.Data.Symbol.Quote.CAD.PercentChange24h ?? 0.0M;
                    cryptoQuoteCAD.PercentChange7d = newQuote.Data.Symbol.Quote.CAD.PercentChange7d ?? 0.0M;
                    cryptoQuoteCAD.PercentChange30d = newQuote.Data.Symbol.Quote.CAD.PercentChange30d ?? 0.0M;
                    cryptoQuoteCAD.PercentChange60d = newQuote.Data.Symbol.Quote.CAD.PercentChange60d ?? 0.0M;
                    cryptoQuoteCAD.PercentChange90d = newQuote.Data.Symbol.Quote.CAD.PercentChange90d ?? 0.0M;
                    cryptoQuoteCAD.MarketCap = newQuote.Data.Symbol.Quote.CAD.MarketCap ?? 0.0;
                    cryptoQuoteCAD.LastUpdated = timeNow;

                    //Update Crypto Object 
                    crypto.Slug = newQuote.Data.Symbol.Slug;
                    crypto.MaxSupply = newQuote.Data.Symbol.MaxSupply;
                    crypto.CirculatingSupply = newQuote.Data.Symbol.CirculatingSupply;
                    crypto.TotalSupply = newQuote.Data.Symbol.TotalSupply;
                    crypto.CmcRank = newQuote.Data.Symbol.CmcRank;
                    crypto.LastUpdated = timeNow;

                    _context.CryptoQuoteUSD.Update(cryptoQuoteUSD);
                    _context.CryptoQuoteCAD.Update(cryptoQuoteCAD);
                    _context.Cryptos.Update(crypto);
                }



                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new AppException("Could not update crypto quote in database");
            }
        }

        public List<CryptoHoldingResponse> GetAllCryptoHoldings(Account account)
        {
            try
            {
                _orderService.UpdateStockAndCryptoHoldings(account, false);
                var allCryptoHoldings = new List<CryptoHoldingResponse>();
                var cryptoHoldings = _context.CryptoHoldings.AsNoTracking().Where(x => x.Account.Id == account.Id).ToList();
                foreach (var holding in cryptoHoldings)
                {
                    //I have to do this because EF Core returns null for the foreign key Crypto
                    var cryptoId = _context.CryptoHoldings.AsNoTracking().Where(x => x.Id == holding.Id).Select(x => x.Crypto).FirstOrDefault();
                    var crypto = _context.Cryptos.Find(cryptoId.Id);
                    var cryptoInfo = _context.CryptoInfo.AsNoTracking().Where(x => x.Crypto.Id == crypto.Id).FirstOrDefault();


                    var cryptoToAdd = _mapper.Map<CryptoHoldingResponse>(holding);
                    cryptoToAdd.CryptoId = crypto.Id;
                    cryptoToAdd.Name = crypto.Name;
                    cryptoToAdd.Symbol = crypto.Symbol;
                    cryptoToAdd.Logo = cryptoInfo.Logo;


                    allCryptoHoldings.Add(cryptoToAdd);
                }


                return allCryptoHoldings;
            }
            catch (Exception)
            {
                throw new AppException("Error Getting User's Crypto Holdings");
            }

        }

        public CryptoHolding GetCryptoHolding(Account account, int cryptoId)
        {
            try
            {
                _orderService.UpdateStockAndCryptoHoldings(account, false);
                var crypto = _context.CryptoHoldings.AsNoTracking().Where(x => x.Account.Id == account.Id && x.Crypto.Id == cryptoId).FirstOrDefault();

                return crypto;
            }
            catch (Exception)
            {
                throw new AppException("Error Getting Crypto in User's Holdings");
            }

        }

        public List<PriceChartResponse> GetTodaysChart(Account account, int cryptoId)
        {
            try
            {
                var crypto = _context.Cryptos.Find(cryptoId);
                var chartResult = _cmcApiService.GetTodaysChart(crypto.Symbol, account.Currency).Result.Data.Quotes;

                List<PriceChartResponse> priceChartResponse = new List<PriceChartResponse>();

                foreach(var quote in chartResult)
                {
                    priceChartResponse.Add(new PriceChartResponse{
                        Date = quote.Quote.CurrencyQuote.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        Close = quote.Quote.CurrencyQuote.Close
                    });
                }

                return priceChartResponse;
            }
            catch (Exception)
            {
                throw new AppException("Error Getting Crypto Chart for Today");
            }

        }

        public List<PriceChartResponse> GetHistoricalChart(Account account, int cryptoId)
        {
            try
            {
                var crypto = _context.Cryptos.Find(cryptoId);
                var chartResult = _cmcApiService.GetHistoricalChart(crypto.Symbol, account.Currency).Result.Data.Quotes;

                List<PriceChartResponse> priceChartResponse = new List<PriceChartResponse>();

                foreach (var quote in chartResult)
                {
                    priceChartResponse.Add(new PriceChartResponse
                    {
                        Date = quote.Quote.CurrencyQuote.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        Close = quote.Quote.CurrencyQuote.Close
                    });
                }

                return priceChartResponse;
            }
            catch (Exception)
            {
                throw new AppException("Error Getting Crypto Historical Chart");
            }

        }
    }
}