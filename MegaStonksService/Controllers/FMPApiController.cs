using MegaStonksService.Entities.Assets;
using MegaStonksService.Models.FinancialModellingPrepApi;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FMPApiController : BaseController
    {
        private readonly IFMPApiService _fmpApiService;

        public FMPApiController(IFMPApiService fmpApiService)
        {
            _fmpApiService = fmpApiService;
        }

        [HttpGet("stocklist")]
        public ActionResult<IEnumerable<StockModel>> GetStocksList(string country)
        {
            var result = _fmpApiService.GetStocksListAsync(country).Result;
            return Ok(result);

        }

        [HttpGet("convertcurrency")]
        public ActionResult<IEnumerable<StockModel>> ConvertCurrency(decimal amount, string currencyPair)
        {
            var result = _fmpApiService.ConvertCurrency(amount, currencyPair).Result;
            return Ok(result);

        }
        [HttpGet("stockprofile")]
        public ActionResult<IEnumerable<StockProfileModel>> GetStockProfile(int stockId)
        {
            var result = _fmpApiService.GetStockProfile(stockId).Result;
            return Ok(result);

        }
        [HttpGet("stockquote")]
        public ActionResult<IEnumerable<StockModel>> GetStockQuote(int stockId)
        {
            var result = _fmpApiService.GetStockQuote(stockId).Result;
            return Ok(result);

        }

        [HttpGet("pricechart")]
        public ActionResult<IEnumerable<PriceChartModel>> GetPriceChart(int stockId)
        {
            var result = _fmpApiService.GetTodaysChart(stockId).Result;
            return Ok(result);

        }

        [HttpGet("pricehistory")]
        public ActionResult<IEnumerable<HistoricalPriceChartModel>> GetHistoryChart(int stockId)
        {
            var result = _fmpApiService.GetHistoricalChart(stockId).Result;
            return Ok(result);

        }

        [HttpGet("IsmarketOpen")]
        public ActionResult<bool> GetMarketStatus()
        {
            var result = _fmpApiService.IsMarketOpen().Result;
            return Ok(result);

        }

        [HttpGet("news")]
        public ActionResult<List<StockNewsModel>> GetNews()
        {
            var result = _fmpApiService.GetNews().Result;
            return Ok(result);

        }
    }
}
