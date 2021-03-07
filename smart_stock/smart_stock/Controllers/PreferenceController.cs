using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;
using System.Collections.Generic;
using System.Linq;

namespace smart_stock.Controllers
{
    [Route("api/preference")]
    [ApiController]
    [EnableCors("preference")]
    public class PreferenceController : ControllerBase
    {
        private readonly IPreferenceProvider _preferenceProvider;
        public PreferenceController (IPreferenceProvider preferenceProvider)
        {
            _preferenceProvider = preferenceProvider;
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