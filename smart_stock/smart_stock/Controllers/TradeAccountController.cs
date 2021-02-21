using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;
using System;
using System.ComponentModel;

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
            Console.WriteLine("GET TA" + id);
            var tas = await _portfolioProvider.GetTradeAccounts(id);
            return tas.ToList();
        }

        // TODO- this is generating a template trade account for portfolio. In future replace portfolio id param with real trade account object
        // POST: api/portfolio/tradeaccount
        [HttpPost("{id}")]
        public async Task<ActionResult<bool>> PostPaymentDetail(int id)
        {
            Console.WriteLine("POST" + id);
            return await _portfolioProvider.InsertTradeAccount(id);
        }
    }
}