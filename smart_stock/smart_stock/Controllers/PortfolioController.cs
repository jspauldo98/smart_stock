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
    [Route("api/portfolio")]
    [ApiController]
    [EnableCors("portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioProvider _portfolioProvider;

        public PortfolioController(IPortfolioProvider portfolioProvider)
        {
            _portfolioProvider = portfolioProvider;
        }

        // GET: api/Portfolio/id        
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(int id)
        {
            Console.WriteLine("GET" + id);
            var portfolio = await _portfolioProvider.GetPortfolio(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            Console.WriteLine(portfolio.Profit);

            return portfolio;
        }

        // // PUT: api/Portfolio/id
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutUser(int id, Portfolio p)
        // {
        //     if (id != p.Id)
        //     {
        //         return BadRequest();
        //     }

        //     if (_portfolioProvider.PortfolioExists(id))
        //     {
        //         await _portfolioProvider.UpdatePortfolio(id, p);
        //     }  
        //     else 
        //     {
        //         return NotFound();
        //     }          

        //     return NoContent();
        // }

        // // POST: api/Portfolio
        // [HttpPost]
        // public async Task<ActionResult<bool>> PostPortfolio(Portfolio p)
        // {
        //     return await _portfolioProvider.InsertPortfolio(p);
        // }

        // // DELETE: api/Portfolio/id
        // [HttpDelete("{id}")]
        // public async Task<ActionResult<bool>> DeletePortfolio(int id)
        // {
        //     var portfolio = await _portfolioProvider.DeletePortfolio(id);
        //     if (!portfolio)
        //     {
        //         return NotFound();
        //     }

        //     return portfolio;
        // }
    }
}