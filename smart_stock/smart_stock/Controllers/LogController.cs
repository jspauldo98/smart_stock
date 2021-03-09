using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;

namespace smart_stock.Controllers
{

    [Route("api/portfolio/tradeaccount/log")]
    [ApiController]
    [EnableCors("portfolio/tradeaccount/log")]
    public class LogController : ControllerBase
    {
        private readonly ILogProvider _logProvider;

        public LogController(ILogProvider logProvider)
        {
            _logProvider = logProvider;
        }

        // GET: api/portfolio/tradeaccount/log/id
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLog(int id)
        {
            var log = await _logProvider.GetLog(id);
            return log.ToList();
        }
    }
}