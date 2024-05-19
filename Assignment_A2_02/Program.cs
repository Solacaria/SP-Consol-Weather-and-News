using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Assignment_A2_02.Models;
using Assignment_A2_02.Services;


namespace Assignment_A2_02
{
    class Program
    {
        static void Main(string[] args)
        {

            NewsService service = new NewsService();
            service.NewsEventPrinter += NewsSub;

            Task<News>[] Tasks = { null, null, null, null, null, null, null };
            Exception exception = null;
            try
            {
                int i = 0;
                foreach (var task in Tasks)
                {
                    NewsCategory nc = (NewsCategory)i;
                    Tasks[i] = service.GetNewsAsync(nc);
                    i++;
                }
                
                Task.WaitAll(Tasks);
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
                    Console.WriteLine("-------------------------------------------\n");
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
