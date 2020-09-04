using System;
//This is an example of how all of our server-side data models should look. Keep everything organized, or it will get out of hand fast.
namespace smart_stock.Models   
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public string DateOfForecast { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556); 

        public string Summary { get; set; }
    }
}
