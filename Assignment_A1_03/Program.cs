using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;
using Assignment_A1_03.Services;

namespace Assignment_A1_03
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += WeatherSub;

            //Register the event
            //Your Code

            Console.Write("Pick a city you want to get weatherdata from: ");
            string? city = Console.ReadLine();
            Console.Clear();

            Task<Forecast>[] tasks = { null, null, null, null };
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync(city);

                Task.WaitAll(tasks[0], tasks[1]);

                tasks[2] = service.GetForecastAsync(latitude, longitude);
                tasks[3] = service.GetForecastAsync(city);

                //Wait and confirm we get an event showing cahced data avaialable
                Task.WaitAll(tasks[2], tasks[3]);
            }
            catch (Exception ex)
            {
                exception = ex;
                Console.WriteLine("Missing information, did you select proper coordinates and/or city name? " + exception);
                //How to handle an exception
                //Your Code
            }
            foreach (var task in tasks)
            {
                //How to deal with successful and fault tasks
                //Your Code
                if (task != null && task.Status == TaskStatus.RanToCompletion)
                {
                    var groupedList = task.Result.Items.ToList().GroupBy(x => x.DateTime.Date);

                    Console.WriteLine("");
                    Console.WriteLine($"Weather status for: {task.Result.City}");
                    foreach (var item in groupedList)
                    {
                        Console.WriteLine($"{item.Key.ToString("yyyy-MM-dd")}");
                        foreach (var item2 in item)
                        {
                            Console.WriteLine(item2);
                        }
                    }
                }
                else if (task != null && task.Status == TaskStatus.Faulted)
                {
                    Console.WriteLine($"An error occured when trying to fetch data: {exception.Message}");
                }
            }
        }
        //Event handler declaration
        //Your Code
        public static void WeatherSub(object sender, string e)
        {
            
            Console.WriteLine("Event message: " + e);
        }

      
    }
}
