using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Assignment_A2_04.Models;
using Assignment_A2_04.Services;

namespace Assignment_A2_04
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsService service = new NewsService();
            service.NewsEventPrinter += NewsSub;

            Task<News>[] Tasks = { null, null, null, null, null, null, null, null, null, null, null, null, null, null };
            Exception exception = null;

            try
            {
                for (int i = 0; i <= 6; i++)
                {
                    NewsCategory nc = (NewsCategory)i;
                    Tasks[i] = service.GetNewsAsync(nc);
                }
                Task.WaitAll(Tasks[0], Tasks[1], Tasks[2], Tasks[3], Tasks[4]);

                for (int i = 7; i <= 13; i++)
                {
                    NewsCategory nc = (NewsCategory)i - 7;
                    Tasks[i] = service.GetNewsAsync(nc);
                }
                Task.WaitAll(Tasks[5], Tasks[6], Tasks[7], Tasks[8], Tasks[9]);
            }
            catch (Exception ex)
            {
                exception = ex;
                Console.WriteLine("An unexpected error occured with: " + exception);
            }
            foreach (var task in Tasks)
            {
                if (task != null && task.Status == TaskStatus.RanToCompletion)
                {
                    var groupedList = task.Result.Articles.ToList();
                    Console.WriteLine($"\nNewscategory: {task.Result.Category}");
                    Console.WriteLine("------------------------------------------------------------------------");
                    foreach (var item in groupedList)
                    {
                        
                        Console.WriteLine($"{item.Title} {item.Description}");
                    }
                }
                else if (task != null && task.Status == TaskStatus.Faulted)
                {
                    Console.WriteLine($"An error occured when trying to fetch data: {exception.Message}");
                }
            }
        }
        public static void NewsSub(object sender, string e)
        {
            Console.WriteLine("Event message from service: " + e);
        }
    }
}
