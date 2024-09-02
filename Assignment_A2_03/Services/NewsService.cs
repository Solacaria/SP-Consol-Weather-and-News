//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Assignment_A2_03.Models;
using Assignment_A2_03.ModelsSampleData;
using System.Net;

namespace Assignment_A2_03.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
        ConcurrentDictionary<NewsCategory, News> CachedNewsSerivce = new ConcurrentDictionary<NewsCategory, News>();
        // Your API Key
        readonly string apiKey = "7b61bedd353040fa8829ded067c3b01a";
    

        #region Stuff
        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }
        public event EventHandler<string> NewsEventPrinter;
        protected virtual void OnNewsEventPrinter(string msg)
        {
            NewsEventPrinter?.Invoke(this, msg);
        }
        #endregion
        public async Task<News> GetNewsAsync(NewsCategory category)
        {
#if UseNewsApiSample      
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category.ToString());

         

#else
           
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}";

            // make the http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            //Convert Json to Object
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();
#endif
            var date = DateTime.Now.ToString("yyyy-MM-dd hh:MM");

            if (CachedNewsSerivce.ContainsKey(category))
            {
                News ns = CachedNewsSerivce[category];
                OnNewsEventPrinter($"Cached news report from the following category: {category} at {date}");
                return ns;
            }
            var news = new News()
            {
                Category = category,
                Articles = nd.Articles.Select(x => new NewsItem
                {
                    Title = x.Title,
                    Description = x.Description,
                    Url = x.Url
                }).ToList(),
            };

            CachedNewsSerivce[category] = news;
            OnNewsEventPrinter($"News report from category: {category}");

            return news;
        }
    }
}
