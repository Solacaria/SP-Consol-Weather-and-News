using System;


namespace Assignment_A1_01.Models
{
    public class ForecastItem
    {
        public DateTime DateTime { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public override string ToString() => $"Time: {DateTime:HH:MM}," +
            $" Temperature: {Temperature.ToString().PadRight(5)} ºC," +
            $" Windspeed: {WindSpeed.ToString().PadRight(5)} m/s," +
            $" Description: {Description}";

    }
    /*
     $"Time: {DateTime}," +
            $" Temperature: {Temperature.ToString().PadRight(5)} ºC," +
            $" Windspeed: {WindSpeed.ToString().PadRight(5)} m/s," +
            $" Description: {Description}";
     */
    /*
    $"Temperature: {Temperature.ToString().PadRight(5)} ºC," +
            $" Windspeed: {WindSpeed.ToString().PadRight(5)} m/s," +
            $" Description: {Description}";

    */
}
