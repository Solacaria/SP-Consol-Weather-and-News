using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string fileName = "Prognos.json";
            double latitude = 59.5086798659495;
            double longitude = 18.2654625932976;

            Forecast forecast = new Forecast();

            if (File.Exists(fname(fileName)))
            {
                forecast = ReadFromDisk(fileName);
            }
            else
            {
                forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);
            }

            //Your Code to present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");
            var groupedList = forecast.Items.ToList().GroupBy(x => x.DateTime.Date);

            foreach (var item in groupedList)
            {
                Console.WriteLine($"{item.Key.ToString("yyyy-MM-dd")}");               
                foreach (var item2 in item)
                {
                    Console.WriteLine(item2);
                }
            }
            //WriteToDisk(fileName, forecast);
        }
        public static void WriteToDisk(string fileName, Forecast fc)
        {
            using (Stream s = File.Create(fname(fileName)))
            using (TextWriter writer = new StreamWriter(s))
            {
                var json = JsonSerializer.Serialize(fc);
                writer.Write(json);
            }
        }
        public static Forecast ReadFromDisk(string fileName)
        {
            Forecast fc;
            using (Stream s = File.OpenRead(fname(fileName)))
            using (TextReader reader = new StreamReader(s))
            {
                var Json = reader.ReadToEnd();
                fc = JsonSerializer.Deserialize<Forecast>(Json);
            }
            return fc;
        }
        static string fname(string name)
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documentPath = Path.Combine(documentPath, "Weather reports");
            if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
            return Path.Combine(documentPath, name);
        }
    }
}
