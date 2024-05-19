using System;

namespace Assignment_A1_03.Models
{
    public class ForecastItem
    {
        public DateTime DateTime { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public override string ToString() => $"  -{DateTime:HH:MM}," +
        $" Temperature: {Temperature.ToString().PadRight(7)}ºC" +
        $" Windspeed: {WindSpeed.ToString().PadRight(7)} m/s" +
        $" Condition: {Description}";
    }
}
