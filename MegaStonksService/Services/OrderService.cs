using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.CoinMarketCapApi;
using MegaStonksService.Services.CoinMarketCapApi;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Services
{
    public interface IOrderService
    {
        OrderStockResponseModel OrderStock(OrderStockModel order, Account account);
        void UpdateStockAndCryptoHoldings(Account account, bool didHoldingsChange);
        List<OrdersResponse> GetAllStockOrders(Account account);
        OrderCryptoResponseModel OrderCrypto(OrderCryptoModel order, Account account);
        List<OrderCryptoResponseModel> GetAllCryptoOrders(Account account);

    }
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly IFMPApiService _fmpApiService;
        private readonly ICMCApiService _cmcApiService;
        private readonly IMapper _mapper;
        private readonly decimal _commisionFees = 10.0M; //Rename to Stock Commision Fees

        public OrderService(DataContext context, IFMPApiService fmpApiService, ICMCApiService cmcApiService, IMapper mapper)
        {
            _context = context;
            _fmpApiService = fmpApiService;
            _cmcApiService = cmcApiService;
            _mapper = mapper;
        }
        public void CheckIfMarketOpen()
        {
            bool marketOpen = _fmpApiService.IsMarketOpen().Result;
            if (!marketOpen)
            {
                throw new AppException("The Stock Market is Currently Closed");
            }
        }

        public OrderStockResponseModel OrderStock(OrderStockModel order, Account account)
        {
            try
            {
                //CheckIfMarketOpen();
                var stockHolding = _context.StockHoldings.Where(x => x.Account.Id == account.Id
                     && x.Stock.Id == order.StockId).SingleOrDefault();
                var stock = _context.Stocks.Find(order.StockId);
                var stockProfile = _fmpApiService.GetStockProfile(order.StockId).Result;
                var userWallet = _context.Wallets.Where(x => x.Account.Id == account.Id).SingleOrDefault();
                var orderStockResponse = new OrderStockResponseModel();

                if (!stockProfile.IsActivelyTrading)
                {
                    throw new AppException("This asset is not tradable at this time");
                }

                decimal orderCost = _commisionFees;
                decimal forexExchangeRate = 1.0M;
                decimal exchangeResult = 0.0M;


                if (stockProfile.Currency != account.Currency)
                {
                    //Convert needed unds from wallet to stock Currency
                    if (account.Currency == Currency.CAD.ToString("G")
                        && stockProfile.Currency == Currency.USD.ToString("G"))
                    {
                        forexExchangeRate = _fmpApiService.ConvertCurrency(1.0M, "USDCAD").Result.Rate;
                        exchangeResult = order.QuantitySubmitted * (forexExchangeRate * stockProfile.Price);

                        orderCost += exchangeResult;
                    }
                    else if (account.Currency == Currency.USD.ToString("G")
                        && stockProfile.Currency == Currency.CAD.ToString("G"))
                    {
                        forexExchangeRate = _fmpApiService.ConvertCurrency(stockProfile.Price, "CADUSD").Result.Rate;
                        exchangeResult = order.QuantitySubmitted * (forexExchangeRate * stockProfile.Price);
                        orderCost += exchangeResult;
                    }

                }
                else
                {
                    orderCost += (stockProfile.Price * order.QuantitySubmitted);

                }


                if (order.OrderType == OrderType.MarketOrder.ToString("G")
                    && order.OrderAction == OrderAction.Buy.ToString("G") && order.QuantitySubmitted > 0.0)
                {


                    if (orderCost > userWallet.CurrentAmount)
                    {
                        throw new AppException("You Cannot Afford this Trade at this Time. Please try a Different Quantity");
                    }

                    var timeUtc = DateTime.UtcNow;

                    var orderEntity = new Order
                    {
                        Account = account,
                        Stock = stock,
                        OrderType = order.OrderType,
                        OrderStatus = OrderStatus.Executed.ToString("G"),
                        OrderAction = OrderAction.Buy.ToString("G"),
                        QuantitySubmitted = order.QuantitySubmitted,
                        QuantityFilled = order.QuantitySubmitted,
                        Commission = _commisionFees,
                        PricePerShare = (stockProfile.Price * forexExchangeRate),
                        TotalPriceFilled = (stockProfile.Price * order.QuantitySubmitted * forexExchangeRate),
                        TotalCost = orderCost,
                        DateSubmitted = timeUtc,
                        DateFilled = timeUtc,
                        LastUpdated = timeUtc
                    };


                    userWallet.CurrentAmount -= orderCost;

                    //Add Stock to User's Stock holdings if the stock does not Exist 

                    if (stockHolding == null)
                    {
                        var newStockholding = new StockHolding
                        {
                            Account = account,
                            Stock = stock,
                            AverageCost = orderEntity.PricePerShare,
                            Quantity = orderEntity.QuantityFilled,
                            MarketValue = orderEntity.QuantityFilled * stockProfile.Price * forexExchangeRate,
                            LastUpdated = timeUtc
                        };

                        _context.StockHoldings.Add(newStockholding);

                    }
                    else
                    {
                        //ReCalculate Average Cost 
                        var newAverageCost = ((stockHolding.AverageCost * stockHolding.Quantity) + (orderEntity.PricePerShare * orderEntity.QuantityFilled)) / (stockHolding.Quantity + orderEntity.QuantityFilled);
                        stockHolding.Quantity += orderEntity.QuantityFilled;
                        stockHolding.AverageCost = newAverageCost;
                        stockHolding.LastUpdated = timeUtc;
                        _context.StockHoldings.Update(stockHolding);
                    }



                    _context.Orders.Add(orderEntity);
                    _context.Wallets.Update(userWallet);

                    _context.SaveChanges();

                    UpdateStockAndCryptoHoldings(account, true);

                    orderStockResponse = _mapper.Map<OrderStockResponseModel>(orderEntity);
                }
                else if (order.OrderType == OrderType.MarketOrder.ToString("G")
                         && order.OrderAction == OrderAction.Sell.ToString("G") && order.QuantitySubmitted > 0.0)
                {
                    if (stockHolding == null || order.QuantitySubmitted > stockHolding.Quantity)
                    {
                        throw new AppException("No Shorts Selling is Allowed at this Time.");
                    }

                    var timeUtc = DateTime.UtcNow;

                    var orderEntity = new Order
                    {
                        Account = account,
                        Stock = stock,
                        OrderType = order.OrderType,
                        OrderStatus = OrderStatus.Executed.ToString("G"),
                        OrderAction = OrderAction.Sell.ToString("G"),
                        QuantitySubmitted = order.QuantitySubmitted,
                        QuantityFilled = order.QuantitySubmitted,
                        Commission = _commisionFees,
                        PricePerShare = (stockProfile.Price * forexExchangeRate),
                        TotalPriceFilled = (stockProfile.Price * order.QuantitySubmitted * forexExchangeRate),
                        TotalCost = orderCost,
                        DateSubmitted = timeUtc,
                        DateFilled = timeUtc,
                        LastUpdated = timeUtc
                    };
                    //20 10 2
                    userWallet.CurrentAmount += (orderCost - _commisionFees);
                    userWallet.CurrentAmount -= _commisionFees;
                    //28 or 26
                    //Update stockHolding for the Stock 

                    stockHolding.Quantity -= orderEntity.QuantityFilled;
                    stockHolding.LastUpdated = timeUtc;


                    

                    if (stockHolding.Quantity <= 0)
                    {
                        _context.StockHoldings.Remove(stockHolding);

                    }
                    else
                    {
                         _context.StockHoldings.Update(stockHolding);
                    }


                    _context.Orders.Add(orderEntity);
                    _context.Wallets.Update(userWallet);

                    _context.SaveChanges();

                    UpdateStockAndCryptoHoldings(account, true);

                    orderStockResponse = _mapper.Map<OrderStockResponseModel>(orderEntity);
                }
                else
                {
                    throw new AppException("Invalid Order");
                }

                orderStockResponse.StockSymbol = stock.Symbol;
                orderStockResponse.Name = stock.CompanyName;
                orderStockResponse.Currency = stockProfile.Currency;
                orderStockResponse.ForexExchangeRate = forexExchangeRate;
                orderStockResponse.ExchangeResult = exchangeResult;
             
                return orderStockResponse;
            }
            catch (Exception)
            {
                throw;
            }

        }


        public OrderCryptoResponseModel OrderCrypto(OrderCryptoModel order, Account account)
        {
            try
            {
                var cryptoHolding = _context.CryptoHoldings.Where(x => x.Account.Id == account.Id
                     && x.Crypto.Id == order.CryptoId).SingleOrDefault();
                var crypto = _context.Cryptos.Find(order.CryptoId);
                var cryptoInfo = _context.CryptoInfo.AsNoTracking().Where(x => x.Crypto.Id == crypto.Id).SingleOrDefault();
                var cryptoQuote = new QuoteModel();
                var orderCryptoResponse = new OrderCryptoResponseModel();

                if (account.Currency == Currency.USD.ToString("G"))
                {
                    cryptoQuote = _cmcApiService.GetLatestCryptoQuote(crypto.Symbol).Result.Data.Symbol.Quote.USD;
                }
                else if(account.Currency == Currency.CAD.ToString("G"))
                {
                    cryptoQuote = _cmcApiService.GetLatestCryptoQuote(crypto.Symbol).Result.Data.Symbol.Quote.CAD;
                }
                    
                
                var userWallet = _context.Wallets.Where(x => x.Account.Id == account.Id).SingleOrDefault();

                decimal orderCost = 0.0M;

                orderCost += (cryptoQuote.Price * order.QuantitySubmitted);




                if (order.OrderType == OrderType.MarketOrder.ToString("G")
                    && order.OrderAction == OrderAction.Buy.ToString("G") && order.QuantitySubmitted > 0.0M)
                {


                    if (orderCost > userWallet.CurrentAmount)
                    {
                        throw new AppException("You Cannot Afford this Trade at this Time. Please try a Different Quantity");
                    }



                    //Get Current Time in EST Zone
                    var timeUtc = DateTime.UtcNow;

                    var orderEntity = new CryptoOrder
                    {
                        Account = account,
                        Crypto = crypto,
                        OrderType = order.OrderType,
                        OrderStatus = OrderStatus.Executed.ToString("G"),
                        OrderAction = OrderAction.Buy.ToString("G"),
                        QuantitySubmitted = order.QuantitySubmitted,
                        QuantityFilled = order.QuantitySubmitted,
                        Commission = 0.0M,
                        PricePerShare = cryptoQuote.Price,
                        TotalPriceFilled = (cryptoQuote.Price * order.QuantitySubmitted),
                        TotalCost = orderCost,
                        DateSubmitted = timeUtc,
                        DateFilled = timeUtc,
                        LastUpdated = timeUtc
                    };


                    userWallet.CurrentAmount -= orderCost;

                    //Add Stock to User's Stock holdings if the stock does not Exist 

                    if (cryptoHolding == null)
                    {
                        var newCryptoHolding = new CryptoHolding
                        {
                            Account = account,
                            Crypto = crypto,
                            AverageCost = orderEntity.PricePerShare,
                            Quantity = orderEntity.QuantityFilled,
                            MarketValue = (orderEntity.QuantityFilled * cryptoQuote.Price),
                            LastUpdated = timeUtc
                        };

                        _context.CryptoHoldings.Add(newCryptoHolding);

                    }
                    else
                    {
                        //ReCalculate Average Cost 
                        var newAverageCost = ((cryptoHolding.AverageCost * cryptoHolding.Quantity) + (orderEntity.PricePerShare * orderEntity.QuantityFilled)) / (cryptoHolding.Quantity + orderEntity.QuantityFilled);
                        cryptoHolding.Quantity += orderEntity.QuantityFilled;
                        cryptoHolding.AverageCost = newAverageCost;
                        cryptoHolding.LastUpdated = timeUtc;
                        _context.CryptoHoldings.Update(cryptoHolding);
                    }



                    _context.CryptoOrders.Add(orderEntity);
                    _context.Wallets.Update(userWallet);

                    _context.SaveChanges();

                    UpdateStockAndCryptoHoldings(account, true);

                    orderCryptoResponse = _mapper.Map<OrderCryptoResponseModel>(orderEntity);

                }
                else if (order.OrderType == OrderType.MarketOrder.ToString("G")
                         && order.OrderAction == OrderAction.Sell.ToString("G") && order.QuantitySubmitted > 0.0M)
                {
                    if (cryptoHolding == null || order.QuantitySubmitted > cryptoHolding.Quantity)
                    {
                        throw new AppException("No Shorts Selling is Allowed at this Time.");
                    }

                    //Get Current Time in UTC Time Zone
                    var timeUtc = DateTime.UtcNow;

                    var orderEntity = new CryptoOrder
                    {
                        Account = account,
                        Crypto = crypto,
                        OrderType = order.OrderType,
                        OrderStatus = OrderStatus.Executed.ToString("G"),
                        OrderAction = OrderAction.Sell.ToString("G"),
                        QuantitySubmitted = order.QuantitySubmitted,
                        QuantityFilled = order.QuantitySubmitted,
                        Commission = 0.0M,
                        PricePerShare = cryptoQuote.Price,
                        TotalPriceFilled = (cryptoQuote.Price * order.QuantitySubmitted),
                        TotalCost = orderCost,
                        DateSubmitted = timeUtc,
                        DateFilled = timeUtc,
                        LastUpdated = timeUtc
                    };
                    //20 10 2
                    userWallet.CurrentAmount += orderCost;
                    //28 or 26
                    //Update stockHolding for the Stock 

                    cryptoHolding.Quantity -= orderEntity.QuantityFilled;
                    cryptoHolding.LastUpdated = timeUtc;




                    if (cryptoHolding.Quantity <= 0)
                    {
                        _context.CryptoHoldings.Remove(cryptoHolding);

                    }
                    else
                    {
                        _context.CryptoHoldings.Update(cryptoHolding);
                    }


                    _context.CryptoOrders.Add(orderEntity);
                    _context.Wallets.Update(userWallet);

                    _context.SaveChanges();

                    UpdateStockAndCryptoHoldings(account, true);

                    orderCryptoResponse = _mapper.Map<OrderCryptoResponseModel>(orderEntity);
                }
                else
                {
                    throw new AppException("Invalid Order");
                }

                orderCryptoResponse.CryptoSymbol = crypto.Symbol;
                orderCryptoResponse.Name = crypto.Name;
                orderCryptoResponse.Logo = cryptoInfo.Logo;
                orderCryptoResponse.Currency = account.Currency;
                orderCryptoResponse.ForexExchangeRate = 1.0M;
                orderCryptoResponse.ExchangeResult = 0.0M;

                return orderCryptoResponse;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //Needs to be called when user request for a Stock as well not just when updating the stockHoldings
        public void UpdateStockAndCryptoHoldings(Account account, bool didHoldingsChange)
        {
            try
            {
                var stockHoldings = _context.StockHoldings.Where(x => x.Account.Id == account.Id).Select(x => x).ToList();
                var cryptoHoldings = _context.CryptoHoldings.Where(x => x.Account.Id == account.Id).Select(x => x).ToList();
                
                decimal portfolioTotal = 0.0M;

                var timeFiteenMinutesAgo = DateTime.UtcNow.AddMinutes(-15);

                if ((stockHoldings.Count <= 0 || stockHoldings == null
                    || (!didHoldingsChange && (stockHoldings.FirstOrDefault().LastUpdated > timeFiteenMinutesAgo)))
                    &&
                    (cryptoHoldings.Count <= 0 || cryptoHoldings == null
                    || (!didHoldingsChange && (cryptoHoldings.FirstOrDefault().LastUpdated > timeFiteenMinutesAgo)))

                    )
                {
                    return;
                }


                if (stockHoldings != null)
                {
                    //Update Returns Columns
                    foreach (var stock in stockHoldings)
                    {
                        //I have to do this because for some reason, Entity Framework returns null for the Stock Object even though it is not Null
                        var stockId = _context.StockHoldings.AsNoTracking().Where(x => x.Id == stock.Id).Select(x => x.Stock).FirstOrDefault();

                        //Could use the API's batch request here for multiple quotes in one request. This could speed things up here
                        var stockQuote = _fmpApiService.GetStockQuote(stockId.Id).Result;

                        decimal forexExchangeRate = 1.0M;

                        var stockProfile = _fmpApiService.GetStockProfile2(stockId.Id).Result;

                        if (stockProfile.Currency != account.Currency)
                        {
                            //Convert stock currency to user wallet currency Currency
                            if (account.Currency == Currency.CAD.ToString("G")
                                && stockProfile.Currency == Currency.USD.ToString("G"))
                            {
                                forexExchangeRate = _fmpApiService.ConvertCurrency(1.0M, "USDCAD").Result.Rate;
                            }
                            else if (account.Currency == Currency.USD.ToString("G")
                                && stockProfile.Currency == Currency.CAD.ToString("G"))
                            {
                                forexExchangeRate = _fmpApiService.ConvertCurrency(stockProfile.Price, "CADUSD").Result.Rate;
                            }

                        }
                        else
                        {
                            forexExchangeRate = 1.0M;

                        }


                        stock.MarketValue = stockQuote.Price * stock.Quantity * forexExchangeRate;

                        if (stock.AverageCost == (stockQuote.Price * forexExchangeRate))
                        {
                            stock.PercentReturnToday = 0.0M;
                            stock.MoneyReturnToday = 0.0M;
                        }
                        else
                        {
                            stock.PercentReturnToday = (decimal)stockQuote.ChangesPercentage;
                            stock.MoneyReturnToday = (stock.PercentReturnToday / 100) * (stock.AverageCost * stock.Quantity);
                        }

                        stock.PercentReturnTotal = (((stockQuote.Price * stock.Quantity * forexExchangeRate) - (stock.AverageCost * stock.Quantity)) / (stock.AverageCost * stock.Quantity)) * 100;
                        stock.MoneyReturnTotal = (stock.PercentReturnTotal / 100) * (stock.AverageCost * stock.Quantity);
                        portfolioTotal += stock.MarketValue;
                    }

                }

                if (cryptoHoldings != null)
                {
                    //Update Returns Columns
                    foreach (var crypto in cryptoHoldings)
                    {
                        //I have to do this because for some reason, Entity Framework returns null for the crypto Object even though it is not Null
                        var cryptoId = _context.CryptoHoldings.AsNoTracking().Where(x => x.Id == crypto.Id).Select(x => x.Crypto).FirstOrDefault();
                        var cryptoInCryptoTable = _context.Cryptos.Find(cryptoId.Id);
                        var cryptoQuote = new QuoteModel();
                        //Could use the API's batch request here for multiple quotes in one request. This could speed things up here
                        if (account.Currency == Currency.USD.ToString("G"))
                        {
                          cryptoQuote = _cmcApiService.GetLatestCryptoQuote(cryptoInCryptoTable.Symbol).Result.Data.Symbol.Quote.USD;
                        }
                        else if (account.Currency == Currency.CAD.ToString("G"))
                        {
                          cryptoQuote = _cmcApiService.GetLatestCryptoQuote(cryptoInCryptoTable.Symbol).Result.Data.Symbol.Quote.CAD;
                        }



                        crypto.MarketValue = cryptoQuote.Price * crypto.Quantity;

                        if (crypto.AverageCost == cryptoQuote.Price)
                        {
                            crypto.PercentReturnToday = 0.0M;
                            crypto.MoneyReturnToday = 0.0M;
                        }
                        else
                        {
                            crypto.PercentReturnToday = cryptoQuote.PercentChange24h ?? 0 ;
                            crypto.MoneyReturnToday = (crypto.PercentReturnToday / 100) * (crypto.AverageCost * crypto.Quantity);
                        }

                        crypto.PercentReturnTotal = (((cryptoQuote.Price * crypto.Quantity) - (crypto.AverageCost * crypto.Quantity)) / (crypto.AverageCost * crypto.Quantity)) * 100;
                        crypto.MoneyReturnTotal = (crypto.PercentReturnTotal / 100) * (crypto.AverageCost * crypto.Quantity);
                        portfolioTotal += crypto.MarketValue;
                    }
                }


                var timeUtcnow = DateTime.UtcNow;

                //Update Holdings Diversity

                if(stockHoldings != null)
                {
                    foreach (var stock in stockHoldings)
                    {
                        stock.PercentOfPortfolio = (stock.MarketValue / portfolioTotal) * 100;
                        stock.LastUpdated = timeUtcnow;
                    }

                    _context.StockHoldings.UpdateRange(stockHoldings);
                }

                if (cryptoHoldings != null)
                {
                    foreach (var crypto in cryptoHoldings)
                    {
                        crypto.PercentOfPortfolio = (crypto.MarketValue / portfolioTotal) * 100;
                        crypto.LastUpdated = timeUtcnow;
                    }

                    _context.CryptoHoldings.UpdateRange(cryptoHoldings);
                }

                _context.SaveChanges();
            }
            catch
            {
                throw new AppException("Could not Update Holdings");
            }

        }

        public List<OrdersResponse> GetAllStockOrders(Account account)
        {
            try
            {
                var ordersResponse = new List<OrdersResponse>();
                var orders = _context.Orders.AsNoTracking().Where(x => x.Account.Id == account.Id).Select(x => x).ToList();
                if(orders.Count <= 0 || orders == null)
                {
                    return new List<OrdersResponse>();
                }
                foreach (var order in orders)
                {
                    var orderToAdd = _mapper.Map<OrdersResponse>(order);
                    int stockId = _context.Orders.AsNoTracking().Where(x => x.Id == order.Id).Select(x => x.Stock.Id).FirstOrDefault();
                    var stock = _context.Stocks.AsNoTracking().Where(x => x.Id == stockId).FirstOrDefault();
                    orderToAdd.Symbol = stock.Symbol;
                    orderToAdd.Name = stock.CompanyName;
                    if (order.DateSubmitted != null)
                    {
                        orderToAdd.DateSubmitted = order.DateSubmitted.Value;
                    }

                    if (order.DateFilled != null)
                    {
                        orderToAdd.DateFilled = order.DateFilled.Value;
                    }

                    ordersResponse.Add(orderToAdd);
                }
                return ordersResponse;
            }
            catch
            {
                throw new AppException("Could not get User's Order History");
            }
        }

        public List<OrderCryptoResponseModel> GetAllCryptoOrders(Account account)
        {
            try
            {
                var ordersResponse = new List<OrderCryptoResponseModel>();
                var orders = _context.CryptoOrders.AsNoTracking().Where(x => x.Account.Id == account.Id).Select(x => x).ToList();
                if (orders.Count <= 0 || orders == null)
                {
                    return ordersResponse;
                }
                foreach (var order in orders)
                {
                    var orderToAdd = _mapper.Map<OrderCryptoResponseModel>(order);
                    int cryptoId = _context.CryptoOrders.AsNoTracking().Where(x => x.Id == order.Id).Select(x => x.Crypto.Id).FirstOrDefault();
                    var crypto = _context.Cryptos.AsNoTracking().Where(x => x.Id == cryptoId).FirstOrDefault();
                    var cryptoInfo = _context.CryptoInfo.AsNoTracking().Where(x => x.Crypto.Id == cryptoId).FirstOrDefault();
                    orderToAdd.CryptoSymbol = crypto.Symbol;
                    orderToAdd.Name = crypto.Name;
                    orderToAdd.Logo = cryptoInfo.Logo;

                    ordersResponse.Add(orderToAdd);
                }
                return ordersResponse;
            }
            catch
            {
                throw new AppException("Could not get User's Order History");
            }
        }
    }
}
