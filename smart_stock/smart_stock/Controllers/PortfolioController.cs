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
            var portfolio = await _portfolioProvider.GetPortfolio(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return portfolio;
        }
    }
}