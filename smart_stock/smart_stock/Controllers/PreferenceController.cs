using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;
using System.Collections.Generic;
using System.Linq;
using smart_stock.AlpacaServices;

namespace smart_stock.Controllers
{
    [Route("api/preference")]
    [ApiController]
    [EnableCors("preference")]
    public class PreferenceController : ControllerBase
    {
        private readonly IPreferenceProvider _preferenceProvider;
        private IFirstPaperTrade _firstPaperTrade;
        public PreferenceController (IPreferenceProvider preferenceProvider, IFirstPaperTrade firstPaperTrade)
        {
            _preferenceProvider = preferenceProvider;
            _firstPaperTrade = firstPaperTrade;
        }

        // GET: api/preference
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RiskLevel>>> GetRiskLevels()
        {
            var riskLevels = await _preferenceProvider.GetRiskLevels();
            if (riskLevels == null)
            {
                return NotFound();
            }

            return riskLevels.ToList();
        }

        // POST: api/preference
        [HttpPost]
        public async Task<ActionResult> InsertPreference([FromBody] Preference preference)
        {
            if (ModelState.IsValid)
            {
                string result = await _preferenceProvider.InsertPreference(preference);
                
                if (result != null)
                {
                    string[] args = new string[2];
                    args[0] = "start";
                    args[1] = result;
                    _firstPaperTrade.CommunicateBackgroundWorker(args);
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