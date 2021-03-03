using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;
using System;

namespace smart_stock.Controllers
{
    [Route("api/portfolio/tradeaccount")]
    [ApiController]
    [EnableCors("portfolio/tradeaccount")]
    public class TradeAccountController : ControllerBase
    {
        private readonly IPortfolioProvider _portfolioProvider;

        public TradeAccountController(IPortfolioProvider portfolioProvider)
        {
            _portfolioProvider = portfolioProvider;
        }

        // GET: api/portfolio/tradeAccount/id
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TradeAccount>>> GetTradeAccounts(int id)
        {
            var tas = await _portfolioProvider.GetTradeAccounts(id);
            return tas.ToList();
        }

        // POST: api/portfolio/tradeaccount
        [HttpPost]
        public async Task<ActionResult<bool>> PostTradeAccount([FromBody]TradeAccount ta)
        {
            return await _portfolioProvider.InsertTradeAccount(ta);
        }
    }
}