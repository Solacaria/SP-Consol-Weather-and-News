using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_02.Models;
using Assignment_A1_02.Services;
using System.Security.Cryptography.X509Certificates;

namespace Assignment_A1_02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            service.WeatherForecastAvailable += WeatherSub;
            //Your Code
            
            string city = "Tierp";
            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service?.GetForecastAsync(city);

                Task.WaitAll(tasks[0], tasks[1]);
            }
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception
                Console.WriteLine("Missing information, did you select proper coordinates and/or city name?" + exception);
                //Your Code
            }
          
            finally
            {
                foreach (var task in tasks)
                {
                   

                    if (task != null && task.Status == TaskStatus.RanToCompletion)
                    {
                        var groupedList = task.Result.Items.ToList().GroupBy(x => x.DateTime.Date);
                        Console.WriteLine("\n-------------------------------");
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
                    //How to deal with successful and fault tasks
                    //Your Code
                }
            }
        }
        public static void WeatherSub(object sender, string e)
        {
            Console.WriteLine("Event message: " + e);
        }
        //Event handler declaration
        //Your Code
    }
}