using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Models.Accounts;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.CoinMarketCapApi;
using MegaStonksService.Models.FinancialModellingPrepApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account, AccountResponse>();

            CreateMap<Account, AuthenticateResponse>();

            CreateMap<RegisterRequest, Account>();

            CreateMap<CreateRequest, Account>();

            CreateMap<Stock, StockModel>()
                .ReverseMap();

            CreateMap<Crypto, CryptoModel>()
               .ReverseMap();

            CreateMap<StockQuoteModel, StockInfoResponse>();

            CreateMap<Order, OrderStockResponseModel>();
            CreateMap<StockHolding, StockHoldingResponse>();
            CreateMap<Order, OrdersResponse>();
            CreateMap<PriceChartModel, PriceChartResponse>();
            CreateMap<Historical, PriceChartResponse>();
            CreateMap<CryptoInfo, CryptoInfoResponse>();
            CreateMap<CryptoQuoteUSD, CryptoQuoteResponse>();
            CreateMap<CryptoQuoteCAD, CryptoQuoteResponse>();
            CreateMap<CryptoOrder, OrderCryptoResponseModel>();
            CreateMap<CryptoHolding, CryptoHoldingResponse>();

            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                    ));


        }
    }
}
