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

        // GET api/portfolio/tradeaccount/log/minute/id
        [HttpGet("minute/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetMinuteLog(int id)
        {
            var log = await _logProvider.GetMinuteLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/hour/id
        [HttpGet("hour/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetHourLog(int id)
        {
            var log = await _logProvider.GetHourLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/day/id
        [HttpGet("day/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetDayLog(int id)
        {
            var log = await _logProvider.GetDayLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/week/id
        [HttpGet("week/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetWeekLog(int id)
        {
            var log = await _logProvider.GetWeekLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/month/id
        [HttpGet("month/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetMonthLog(int id)
        {
            var log = await _logProvider.GetMonthLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/year/id
        [HttpGet("year/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetYearLog(int id)
        {
            var log = await _logProvider.GetYearLog(id);
            return log.ToList();
        }
    }
}