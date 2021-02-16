using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;

namespace smart_stock.Controllers
{
    [Route("api/strategies")]
    [ApiController]
    [EnableCors("strategies")]
    public class TradeStrategyController : ControllerBase
    {
        private readonly ITradeStrategiesProvider _tradeStrategiesProvider;
        public TradeStrategyController (ITradeStrategiesProvider tradeStrategiesProvider)
        {
            _tradeStrategiesProvider = tradeStrategiesProvider;
        }

        // POST: api/strategies
        [HttpPost]
        public async Task<ActionResult> InsertTradeStrategy([FromBody] TradeStrategy tradeStrategy)
        {
            if (ModelState.IsValid)
            {
                string result = await _tradeStrategiesProvider.InsertTradeStrategy(tradeStrategy);
                
                if (result == null)
                {
                    return Ok();
                }
                else {
                    return BadRequest(result);
                }
            }
            return StatusCode(500);
        }
    }
}