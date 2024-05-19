//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Assignment_A2_04.Models;
using Assignment_A2_04.ModelsSampleData;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assignment_A2_04.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();

        ConcurrentDictionary<string, News> CachedNewsSerivce = new ConcurrentDictionary<string, News>();

        // Your API Key
        readonly string apiKey = "";
        /* Get API from 
     https://newsapi.org/
       */


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
            
            var nyckel = new NewsCacheKey(category, DateTime.Now);

            if (nyckel.CacheExist && CachedNewsSerivce.Count == 0)
            {
                News savedNews = News.Deserialize(nyckel.FileName);
                OnNewsEventPrinter($"News event from XML File: {nyckel.Key}");
                return savedNews;
            }
            if (CachedNewsSerivce.ContainsKey(nyckel.FileName))
            {
                News ns = CachedNewsSerivce[nyckel.FileName];
                OnNewsEventPrinter($"Cached news report from the following category: {nyckel.Key}");
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
           
            News.Serialize(news, nyckel.FileName);
            CachedNewsSerivce[nyckel.FileName] = news;

            OnNewsEventPrinter($"News report from category: {category}");
            return news;
        }
    }

}
