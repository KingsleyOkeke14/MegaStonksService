using MegaStonksService.Models.CoinMarketCapApi;
using MegaStonksService.Services.CoinMarketCapApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CMCApiController : BaseController
    {
        private readonly ICMCApiService _cmcApiService;

        public CMCApiController(ICMCApiService cmcApiService)
        {
            _cmcApiService = cmcApiService;
        }

        [HttpGet("cryptoList")]
        public ActionResult<CryptoListingsModel> GetCryptoList()
        {
            var result = _cmcApiService.GetCryptoListingAsync().Result;
            return Ok(result);

        }

        [HttpGet("cryptoInfo")]
        public ActionResult<CryptoInfoModel> GetCryptoInfo(string cryptoSymbol)
        {
            var result = _cmcApiService.GetCryptoInfo(cryptoSymbol).Result;
            return Ok(result);
        }

        [HttpGet("pricechart")]
        public ActionResult<CryptoHistoricalModel> GetCryptoChart(string cryptoSymbol, string currency)
        {
            var result = _cmcApiService.GetTodaysChart(cryptoSymbol, currency).Result;
            return Ok(result);
        }

        [HttpGet("pricehistory")]
        public ActionResult<CryptoHistoricalModel> GetCryptoHistoricalChart(string cryptoSymbol, string currency)
        {
            var result = _cmcApiService.GetHistoricalChart(cryptoSymbol, currency).Result;
            return Ok(result);
        }
    }
}