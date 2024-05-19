using System;

using Assignment_A2_01.Models;
using Assignment_A2_01.Services;

namespace Assignment_A2_01
{
    class Program
    {
        static void Main(string[] args)
        {
            
            NewsService service = new NewsService();

            Task<NewsApiData>[] Tasks = {null };
            Exception exception = null;

            try
            {
                Tasks[0] = service.GetNewsAsync();
                Task.WaitAll(Tasks[0]);
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
                    var groupedlist = task.Result.Articles.ToList().GroupBy(x => x.Source);
                    Console.WriteLine("Testeroni");
                    foreach (var item in groupedlist)
                    {
                        foreach (var item2 in item)
                        {
                            Console.WriteLine($"{item2.Title} {item2.Author}");
                        }
                    }
                }
                else if (task != null && task.Status == TaskStatus.Faulted)
                {
                    Console.WriteLine($"An error occured when trying to fetch data: {exception.Message}");
                }

            }
        }
    }
}
