using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using smart_stock.Models;

namespace smart_stock.Controllers
{
    //This controller is meant to show how we should structure all other endpoints/controllers.
    //Obviously, there will be differences depending on design, but this is a good place to show
    //Http Conventions.
    
    [Route("api/forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private static List<WeatherForecast> forecasts = new List<WeatherForecast>();
        private static int idCounter = 0;
        private static System.Random rng = new Random();
        //Create two forecast objects, and have them available for our client to manipulate. Make them static,
        //So that they are not dynamically created each time this controller is called.
        private static WeatherForecast forecastOne = new WeatherForecast
            {
                Id = idCounter++,
                DateOfForecast = DateTime.Now.AddDays(idCounter).ToString(),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };

        private static WeatherForecast forecastTwo = new WeatherForecast
        {
            Id = idCounter++,
            DateOfForecast = DateTime.Now.AddDays(idCounter).ToString(),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            //This will become useful as we need to track more server side issues
            _logger = logger;
        }

        [HttpGet("forecasts")]
        public WeatherForecast[] Get()
        {
            if (!forecasts.Contains(forecastOne) && !forecasts.Contains(forecastTwo)) 
            {
                forecasts.Add(forecastOne);
                forecasts.Add(forecastTwo);
            }
            return forecasts.ToArray();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //Will complain unless we use await operators, but isn't really needed in these examples
            return Ok(forecasts.Where(forecast => forecast.Id == id).FirstOrDefault());
        }

        [HttpPost("add")]
        public async Task<IActionResult> Post([FromBody] WeatherForecast weatherForecast)
        {
            weatherForecast.Id = idCounter++;
            weatherForecast.DateOfForecast = DateTime.Now.ToString();
            forecasts.Add(weatherForecast);
            return CreatedAtAction("Get", new { id = weatherForecast.Id}, weatherForecast);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Put([FromBody] WeatherForecast weatherForecast)
        {
            WeatherForecast updatedForecast = forecasts.Where(e => e.Id == weatherForecast.Id).FirstOrDefault();
            updatedForecast.DateOfForecast = DateTime.Now.ToString();
            updatedForecast.TemperatureC = weatherForecast.TemperatureC;
            updatedForecast.Summary = weatherForecast.Summary;
            //No content to return, and no call for info was made, so a 200 isn't needed.
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            forecasts.RemoveAll(e => e.Id == id);
            return NoContent();
        }
    }
}
