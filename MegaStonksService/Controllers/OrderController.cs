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
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService stockService)
        {
            _orderService = stockService;
        }
        [HttpPost("orderStock")]
        public ActionResult<OrderStockResponseModel> OrderStock(OrderStockModel orderStockModel)
        {
            var result = _orderService.OrderStock(orderStockModel, Account);
            return Ok(result);
        }
        [HttpPost("orderCrypto")]
        public ActionResult<OrderStockResponseModel> OrderCrypto(OrderCryptoModel orderCryptoModel)
        {
            var result = _orderService.OrderCrypto(orderCryptoModel, Account);
            return Ok(result);
        }
        [HttpGet("getOrders")]
        public ActionResult<OrderStockResponseModel> GetAllOrders()
        {
            var result = _orderService.GetAllStockOrders(Account);
            return Ok(result);
        }
        [HttpGet("getCryptoOrders")]
        public ActionResult<IEnumerable<OrderCryptoResponseModel>> GetAllCryptoOrders()
        {
             var result = _orderService.GetAllCryptoOrders(Account);
            return Ok(result);
        }
    }
}