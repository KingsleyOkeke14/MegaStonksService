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
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public ActionResult<WalletResponseModel> GetUserWallet()
        {
            var result = _walletService.GetWallet(Account);
            return Ok(result);

        }

        [Authorize(Role.Admin)]
        [HttpPost("addFunds")]
        public ActionResult<WalletResponseModel> AddFundsToWallet(int amountToAdd)
        {
            var result = _walletService.AddFundsToWallet(Account, amountToAdd);
            return Ok(result);

        }
    }
}
