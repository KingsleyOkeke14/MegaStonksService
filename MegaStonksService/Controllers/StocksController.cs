using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using MegaStonksService.Models.FinancialModellingPrepApi;
using MegaStonksService.Services;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StocksController : BaseController
    {

        private readonly IStocksService _stockService;

        public StocksController(IStocksService stockService)
        {
            _stockService = stockService;
        }

        [Authorize(Role.Admin)]
        [HttpGet("stocksinschema")]
        public ActionResult<IEnumerable<Stock>> GetStocksInSchema()
        {
            var result = _stockService.ListAllStocksInSchema();
            return Ok(result);

        }

        [HttpGet("searchstock")]
        public ActionResult<IEnumerable<Stock>> SearchStock(string stockToSearch)
        {
            var result = _stockService.SearchStock(stockToSearch);
            return Ok(result);

        }

        [Authorize(Role.Admin)]
        [HttpPost("updateStocksInSchema")]
        public ActionResult<IEnumerable<Stock>> UpdateStockstInSchema()
        {
            var result = _stockService.UpdateStocksInSchema(true).Result;
            return Ok(result);

        }

        [Authorize(Role.Admin)]
        [HttpGet("missingStocksInSchema")]
        public ActionResult<IEnumerable<Stock>> MissingStocksInSchema()
        {
            var result = _stockService.UpdateStocksInSchema(false).Result;
            return Ok(result);
        }

        [HttpGet("stockInfo")]
        public ActionResult<StockInfoResponse> GetStockInfoForUser(int stockId)
        {
            var result = _stockService.GetStockInfo(stockId, Account);
            return Ok(result);
        }

        [HttpGet("stockholding")]
        public ActionResult<StockInfoResponse> GetStockHolding(int stockId)
        {
            var result = _stockService.GetStockHolding(Account, stockId);
            return Ok(result);
        }

        [HttpGet("stockholdings")]
        public ActionResult<StockInfoResponse> GettAllStockHoldings()
        {
            var result = _stockService.GetAllStockHoldings(Account);
            return Ok(result);
        }

        [HttpGet("pricechart")]
        public ActionResult<IEnumerable<PriceChartResponse>> GetPriceChart(int stockId)
        {
            var result = _stockService.GetTodaysChart(stockId);
            return Ok(result);

        }

        [HttpGet("pricehistory")]
        public ActionResult<IEnumerable<PriceChartResponse>> GetHistoryChart(int stockId, string interval)
        {
            var result = _stockService.GetHistoricalChart(stockId, interval);
            return Ok(result);

        }

        [HttpGet("stocknews")]
        public ActionResult<IEnumerable<StockNewsModel>> GetStockNews()
        {
            var result = _stockService.GetNews();
            return Ok(result);
        }
    }
}