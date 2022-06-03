using MegaStonksService.Entities.Assets;
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
  
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WatchListController : BaseController
    {
        private readonly IWatchListService _watchListService;
        public WatchListController(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }

        [HttpGet("getWatchList")]
        public ActionResult<IEnumerable<StockInfoResponse>> GetWatchList()
        {
            var result = _watchListService.GetStockWatchList(Account);
            return Ok(result);

        }

        [HttpPut("addStock")]
        public ActionResult<string> AddStockToWatchList(int stockId)
        {
            var result = _watchListService.AddStockToWatchList(Account, stockId);
            if (result.Symbol != null)
            {
                return Ok(new { message = $"{result.Symbol} Has beeen added to the WatchList" });

            }
            return BadRequest(new { message = "Could not Add Stock to WatchList. Please check the Stock Id" });
        }

        [HttpDelete("removeStock")]
        public ActionResult<Stock> RemoveStockFromWatchList(int stockId)
        {
            var result = _watchListService.RemoveStockFromWatchlist(Account, stockId);
            if (result.Symbol != null)
            {
                return Ok(new { message = $"{result.Symbol} Has beeen Removed From the WatchList" });

            }
            return BadRequest(new { message = "Could not Remove Stock from WatchList. Please check the Stock Id" });

        }

        [HttpGet("getCryptoWatchList")]
        public ActionResult<IEnumerable<CryptoResponseModel>> GetCryptoWatchList()
        {
            var result = _watchListService.GetCryptoWatchList(Account);
            return Ok(result);
        }

        [HttpPut("addCrypto")]
        public ActionResult<string> AddCryptoToWatchList(int cryptoId)
        {
            var result = _watchListService.AddCryptoToWatchList(Account, cryptoId);
            if (result.Symbol != null)
            {
                return Ok(new { message = $"{result.Name} Has beeen added to the WatchList" });

            }
            return BadRequest(new { message = "Could not Add Crypto to WatchList. Please check the Crypto Id" });
        }

        [HttpDelete("removeCrypto")]
        public ActionResult<string> RemoveCryptoFromWatchList(int cryptoId)
        {
            var result = _watchListService.RemoveCryptoFromWatchlist(Account, cryptoId);
            if (result.Symbol != null)
            {
                return Ok(new { message = $"{result.Name} Has beeen Removed From the WatchList" });

            }
            return BadRequest(new { message = "Could not Remove Crypto from WatchList. Please check the Crypto Id" });

        }
    }
}