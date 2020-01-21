using System;
using System.ComponentModel.DataAnnotations;

namespace AdviceFirstApi
{
    public class WeatherForecast
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int TemperatureC { get; set; }
        [Required]
        /// <summary>
        /// Temperature in degree 
        /// </summary>
        /// <returns></returns>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        [Required]
        public string Summary { get; set; }
    }
}
