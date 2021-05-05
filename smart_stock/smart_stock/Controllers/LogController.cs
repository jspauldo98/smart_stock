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

    [Route("api/portfolio")]
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
        [HttpGet("tradeaccount/log/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLog(int id)
        {
            var log = await _logProvider.GetLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/minute/id
        [HttpGet("tradeaccount/log/minute/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetMinuteLog(int id)
        {
            var log = await _logProvider.GetMinuteLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/hour/id
        [HttpGet("tradeaccount/log/hour/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetHourLog(int id)
        {
            var log = await _logProvider.GetHourLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/day/id
        [HttpGet("tradeaccount/log/day/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetDayLog(int id)
        {
            var log = await _logProvider.GetDayLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/week/id
        [HttpGet("tradeaccount/log/week/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetWeekLog(int id)
        {
            var log = await _logProvider.GetWeekLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/month/id
        [HttpGet("tradeaccount/log/month/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetMonthLog(int id)
        {
            var log = await _logProvider.GetMonthLog(id);
            return log.ToList();
        }

        // GET api/portfolio/tradeaccount/log/year/id
        [HttpGet("tradeaccount/log/year/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetYearLog(int id)
        {
            var log = await _logProvider.GetYearLog(id);
            return log.ToList();
        }

        // GET api/portfolio/log/minute/id
        [HttpGet("log/minute/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetMinuteLogByPortfolio(int id)
        {
            var log = await _logProvider.GetMinuteLogByPortfolio(id);
            return log.ToList();
        }

        // GET api/portfolio/log/hour/id
        [HttpGet("log/hour/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetHourLogByPortfolio(int id)
        {
            var log = await _logProvider.GetHourLogByPortfolio(id);
            return log.ToList();
        }

        // GET api/portfolio/log/day/id
        [HttpGet("log/day/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetDayLogByPortfolio(int id)
        {
            var log = await _logProvider.GetDayLogByPortfolio(id);
            return log.ToList();
        }

        // GET api/portfolio/log/week/id
        [HttpGet("log/week/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetWeekLogByPortfolio(int id)
        {
            var log = await _logProvider.GetWeekLogByPortfolio(id);
            return log.ToList();
        }

        // GET api/portfolio/log/month/id
        [HttpGet("log/month/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetMonthLogByPortfolio(int id)
        {
            var log = await _logProvider.GetMonthLogByPortfolio(id);
            return log.ToList();
        }

        // GET api/portfolio/log/year/id
        [HttpGet("log/year/{id}")]
        public async Task<ActionResult<IEnumerable<Log>>> GetYearLogByPortfolio(int id)
        {
            var log = await _logProvider.GetYearLogByPortfolio(id);
            return log.ToList();
        }
    }
}