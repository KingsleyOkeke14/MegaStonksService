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
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AdsController : BaseController
    {
        private readonly IAdService _adService;

        public AdsController(IAdService adService)
        {
            _adService = adService;
        }

        [HttpGet("getAllAds")]
        public ActionResult<IEnumerable<Ad>> GetAllAds()
        {
            var result = _adService.GetAllAds();
            return Ok(result);

        }
        [Authorize(Role.Admin)]
        [HttpGet("getAd")]
        public ActionResult<Ad> GetAd(int adToGet)
        {
            var result = _adService.GetSpecificAd(adToGet);
            return Ok(result);

        }
        [Authorize(Role.Admin)]
        [HttpPut("createAd")]
        public ActionResult<string> CreateAd(CreateAdModel adModel)
        {
            var result = _adService.CreateAd(adModel);

            return Ok(new { message = $"Ad with Title {result.Title} for company {result.Company} Has beeen added to the DataBase" });
        }
        [Authorize(Role.Admin)]
        [HttpDelete("removeAd")]
        public ActionResult<string> RemoveAd(int adId)
        {
            var result = _adService.RemoveAd(adId);

            return Ok(new { message = $"Ad with Title {result.Title} for company {result.Company} Has beeen removed from the DataBase" });

        }
    }
}
