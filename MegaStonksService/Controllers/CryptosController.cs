using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Models.Assets;
using MegaStonksService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CryptosController : BaseController
    {
        private readonly ICryptosService _cryptoService;

        public CryptosController(ICryptosService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpGet("cryptosInSchema")]
        public ActionResult<IEnumerable<Crypto>> GetCryptoInSchema()
        {
            var result = _cryptoService.ListAllCryptoInSchema();
            return Ok(result);
        }

        [Authorize(Role.Admin)]
        [HttpGet("missingCryptosInSchema")]
        public ActionResult<IEnumerable<Crypto>> MissingCryptosInSchema()
        {
            var result = _cryptoService.UpdateCryptosInSchema(false).Result;
            return Ok(result);
        }

        [HttpGet("searchCrypto")]
        public ActionResult<IEnumerable<Crypto>> SearchCrypto(string cryptoToSearch)
        {
            var result = _cryptoService.SearchCrypto(cryptoToSearch);
            return Ok(result);
        }

        [Authorize(Role.Admin)]
        [HttpPost("updateCryptosInSchema")]
        public ActionResult<IEnumerable<Crypto>> UpdateCryptosInSchema()
        {
            var result = _cryptoService.UpdateCryptosInSchema(true).Result;
            return Ok(result);
        }

        [Authorize(Role.Admin)]
        [HttpPut("addCrypto")]
        public ActionResult<IEnumerable<Crypto>> AddCryptoToSchema(string cryptoSymbol)
        {
            var result = _cryptoService.AddCryptoToSchema(cryptoSymbol).Result;
            return Ok(result);
        }

        [HttpGet("quote")]
        public ActionResult<IEnumerable<Crypto>> GetLatestQuote(string cryptoSymbol)
        {
            var result = _cryptoService.GetQuote(cryptoSymbol).Result;
            return Ok(result);
        }

        [HttpGet("crypto")]
        public ActionResult<CryptoResponseModel> GetLatestQuote(int cryptoId)
        {
            var result = _cryptoService.GetCrypto(cryptoId, Account).Result;
            return Ok(result);
        }

        [HttpGet("cryptoHolding")]
        public ActionResult<StockInfoResponse> GetCryptoHolding(int cryptoId)
        {
            var result = _cryptoService.GetCryptoHolding(Account, cryptoId);
            return Ok(result);
        }

        [HttpGet("pricechart")]
        public ActionResult<IEnumerable<PriceChartResponse>> GetCryptoChartToday(int cryptoId)
        {
            var result = _cryptoService.GetTodaysChart(Account, cryptoId);
            return Ok(result);
        }

        [HttpGet("pricehistory")]
        public ActionResult<IEnumerable<PriceChartResponse>> GetCryptoChartHistorical(int cryptoId)
        {
            var result = _cryptoService.GetHistoricalChart(Account, cryptoId);
            return Ok(result);
        }

        [HttpGet("cryptoHoldings")]
        public ActionResult<StockInfoResponse> GettAllStockHoldings()
        {
            var result = _cryptoService.GetAllCryptoHoldings(Account);
            return Ok(result);
        }
    }
}