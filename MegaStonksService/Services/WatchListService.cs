using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Services
{
    public interface IWatchListService
    {
        List<StockInfoResponse> GetStockWatchList(Account account);
        Stock AddStockToWatchList(Account account, int stockIdToAdd);
        Stock RemoveStockFromWatchlist(Account account, int stockIdToRemove);
        List<CryptoResponseModel> GetCryptoWatchList(Account account);
        Crypto AddCryptoToWatchList(Account account, int cryptoIdToAdd);
        Crypto RemoveCryptoFromWatchlist(Account account, int cryptoIdToRemove);
    }

    public class WatchListService : IWatchListService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IStocksService _stockService;
        private readonly ICryptosService _cryptosService;
        public WatchListService(DataContext context, IMapper mapper, IStocksService stockService, ICryptosService cryptosService)
        {
            _context = context;
            _mapper = mapper;
            _stockService = stockService;
            _cryptosService = cryptosService;
        }


        public List<StockInfoResponse> GetStockWatchList(Account account)
        {
            
            try
            {
                var stocks = new List<StockInfoResponse>();
                var watchList = _context.WatchLists.AsNoTracking().Where(x => x.Account.Id == account.Id).Select(x => x.Stock).ToList();
                    
                    if (watchList.Count > 0)
                    {
                        foreach (var stock in watchList)
                        {
                            if(stock!= null)
                            {
                                var stockinDB = _context.Stocks.AsNoTracking().Where(x => x.Id == stock.Id).FirstOrDefault();
                                if (stockinDB != null)
                                {
                                var stockInfo = _stockService.GetStockInfo(stockinDB.Id, account);
                                    stocks.Add(stockInfo);
                                }
                            }
                        }

                    }

                return stocks;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not get stocks in Watchlist from Database");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }


        }

        public Stock AddStockToWatchList(Account account, int stockIdToAdd)
        {
            try
            {
                var stockAdded = new Stock();
                var stockToAdd = _context.Stocks.Find(stockIdToAdd);
               
                if(stockToAdd != null)
                {
                    bool isInWatchList = _context.WatchLists.Where(x => x.Stock.Id == stockToAdd.Id && x.Account.Id == account.Id).Any();
                    if (!isInWatchList)
                    {

                        var watchListToAdd = new WatchList()
                        {
                            Account = account,
                            Stock = stockToAdd,
                            DateAdded = DateTime.UtcNow
                        };

                        _context.WatchLists.Add(watchListToAdd);
                        _context.SaveChanges();
                        stockAdded = stockToAdd;
                    }

                }

                return stockAdded;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not add Stock to Watchlist in Database");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }

        }

        public Stock RemoveStockFromWatchlist(Account account, int stockIdToRemove)
        {
            var stockRemoved = new Stock();
            try
            {
                bool isStockInWatchlist = _context.WatchLists.AsNoTracking().Where(x => x.Stock.Id == stockIdToRemove && x.Account.Id == account.Id).Select(x => x).Any();

                if (isStockInWatchlist)
                {
                    var stockToRemove = _context.WatchLists.AsNoTracking().Where(x => x.Stock.Id == stockIdToRemove && x.Account.Id == account.Id).Select(x => x).Single();

                   
                    if (stockToRemove != null)
                    {

                        _context.WatchLists.Remove(stockToRemove);
                        _context.SaveChanges();
                        stockRemoved = _context.Stocks.Find(stockIdToRemove);
                    }
                }


                return stockRemoved;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not remove Stock from Watchlist in Database");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }

        }

        public List<CryptoResponseModel> GetCryptoWatchList(Account account)
        {

            try
            {
                var cryptos = new List<CryptoResponseModel>();
                var watchList = _context.CryptoWatchLists.AsNoTracking().Where(x => x.Account.Id == account.Id).Select(x => x.Crypto).ToList();

                if (watchList.Count > 0)
                {
                    foreach (var crypto in watchList)
                    {
                        if (crypto != null)
                        {
                            var cryptoInDb = _context.Cryptos.AsNoTracking().Where(x => x.Id == crypto.Id).FirstOrDefault();
                            if (cryptoInDb != null)
                            {
                                var cryptoInfo =  _cryptosService.GetCrypto(cryptoInDb.Id, account).Result;
                                cryptos.Add(cryptoInfo);
                            }
                        }
                    }

                }

                return cryptos;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not get Cryptos in Watchlist from Database");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
        }

        public Crypto AddCryptoToWatchList(Account account, int cryptoIdToAdd)
        {
            try
            {
                var cryptoAdded = new Crypto();
                var cryptoToAdd = _context.Cryptos.Find(cryptoIdToAdd);

                if (cryptoToAdd != null)
                {
                    bool isInWatchList = _context.CryptoWatchLists.Where(x => x.Crypto.Id == cryptoToAdd.Id && x.Account.Id == account.Id).Any();
                    if (!isInWatchList)
                    {
                        var timeUtc = DateTime.UtcNow;

                        var watchListToAdd = new CryptoWatchList()
                        {
                            Account = account,
                            Crypto = cryptoToAdd,
                            DateAdded = timeUtc
                        };

                        _context.CryptoWatchLists.Add(watchListToAdd);
                        _context.SaveChanges();
                        cryptoAdded = cryptoToAdd;
                    }
                }
                return cryptoAdded;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not add Crypto to Watchlist in Database");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }

        }

        public Crypto RemoveCryptoFromWatchlist(Account account, int cryptoIdToRemove)
        {
            var cryptoRemoved = new Crypto();
            try
            {
                bool isCryptoInWatchlist = _context.CryptoWatchLists.AsNoTracking().Where(x => x.Crypto.Id == cryptoIdToRemove && x.Account.Id == account.Id).Select(x => x).Any();

                if (isCryptoInWatchlist)
                {
                    var cryptoToRemove = _context.CryptoWatchLists.AsNoTracking().Where(x => x.Crypto.Id == cryptoIdToRemove && x.Account.Id == account.Id).Select(x => x).Single();


                    if (cryptoToRemove != null)
                    {

                        _context.CryptoWatchLists.Remove(cryptoToRemove);
                        _context.SaveChanges();
                        cryptoRemoved = _context.Cryptos.Find(cryptoIdToRemove);
                    }
                }
                return cryptoRemoved;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not remove Crypto from Watchlist in Database");
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }

        }
    }
}
