using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Services
{
    public interface IWalletService
    {
        WalletResponseModel GetWallet(Account account);
        AddFundsResponseModel AddFundsToWallet(Account account, int amountToAdd);
    }
    public class WalletService : IWalletService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IFMPApiService _fmpApiService;

        public WalletService(DataContext context, IMapper mapper, IOrderService orderService, IFMPApiService fMPApiService)
        {
            _context = context;
            _mapper = mapper;
            _orderService = orderService;
            _fmpApiService = fMPApiService;
        }

        public WalletResponseModel GetWallet(Account account)
        {
            try
            {
                
                _orderService.UpdateStockAndCryptoHoldings(account, false);

                var userWallet = _context.Wallets.Where(x => x.Account.Id == account.Id).FirstOrDefault();
                var stockHoldings = _context.StockHoldings.Where(x => x.Account.Id == account.Id).ToList();
                var cryptoHoldings = _context.CryptoHoldings.Where(x => x.Account.Id == account.Id).ToList();
                decimal holdingsMarketValue = 0.0M;
                decimal moneyReturnToday = 0.0M;
                decimal moneyReturnTotal = 0.0M;
                decimal percentReturnToday = 0.0M;
                decimal percentReturnTotal = 0.0M;

               
                if(stockHoldings != null && stockHoldings.Count > 0)
                {
                    foreach (var holding in stockHoldings)
                    {

                        holdingsMarketValue += holding.MarketValue;

                        moneyReturnToday += holding.MoneyReturnToday;
                    }
                }

                if (cryptoHoldings != null && cryptoHoldings.Count > 0)
                {
                    foreach (var holding in cryptoHoldings)
                    {

                        holdingsMarketValue += holding.MarketValue;

                        moneyReturnToday += holding.MoneyReturnToday;
                    }
                }


                //Calculate Percent Returns
                percentReturnToday = (moneyReturnToday /( holdingsMarketValue + userWallet.CurrentAmount)) * 100;
                moneyReturnTotal = ((holdingsMarketValue + userWallet.CurrentAmount) - userWallet.InitialAmount);
                percentReturnTotal = (moneyReturnTotal / userWallet.InitialAmount) * 100;


                var walletResponse = new WalletResponseModel()
                {
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Cash = userWallet.CurrentAmount,
                    InitialDeposit = userWallet.InitialAmount,
                    Investments = holdingsMarketValue,
                    Total = (userWallet.CurrentAmount + holdingsMarketValue),
                    PercentReturnToday = percentReturnToday,
                    MoneyReturnToday = moneyReturnToday,
                    PercentReturnTotal = percentReturnTotal,
                    MoneyReturnTotal = moneyReturnTotal
                };

                return walletResponse;

            }
            catch (Exception)
            {
                throw new AppException("Error Getting User's Wallet Information");
            }
        }


        public AddFundsResponseModel AddFundsToWallet(Account account, int amountToAdd)
        {
            try
            {

                var userWallet = _context.Wallets.Where(x => x.Account.Id == account.Id).FirstOrDefault();
                var currentBalance = userWallet.CurrentAmount;

                userWallet.InitialAmount += amountToAdd;
                userWallet.CurrentAmount += amountToAdd;
                _context.Wallets.Update(userWallet);
                _context.SaveChanges();

                var response = new AddFundsResponseModel()
                {
                    PreviousBalance = currentBalance,
                    AmountAdded = amountToAdd,
                    NewBalance = userWallet.CurrentAmount

                };

                return response;

            }
            catch (Exception)
            {
                throw new AppException("Could Not Add Funds to user's Wallet");
            }
        }
    }
}