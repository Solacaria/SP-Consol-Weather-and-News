using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        
        //Cache declaration
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        // Your API Key
        readonly string apiKey = "b87c954beb6026a081aa85374b8f9c58 ";
  

        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        protected virtual void OnWeatherForecastAvailable (string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code
        
            var date = DateTime.Now.ToString("yyyy-MM-dd hh:MM");
            if (cachedCityForecasts.ContainsKey((City, date)))
            {
                Forecast cacheFC = cachedCityForecasts[(City, date)];
                OnWeatherForecastAvailable($"Cached data from: {City} at {date}");
                return cacheFC;
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);
            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code

            cachedCityForecasts[(City, date)] = forecast;
            OnWeatherForecastAvailable($"Weather forecast from: {City}");

            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code
            var date = DateTime.Now.ToString("yyyy-MM-dd hh:MM"); 
            if (cachedGeoForecasts.ContainsKey((latitude, longitude, date)))
            {
                Forecast cacheFC = cachedGeoForecasts[(latitude, longitude, date)];
                OnWeatherForecastAvailable($"Cached data from coords: {cacheFC.City} at {date}");
                return cacheFC;
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);
            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code

            cachedGeoForecasts[(latitude, longitude, date)] = forecast;
            OnWeatherForecastAvailable($"Weather forecast coords: {forecast.City}");

            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();
            //Convert WeatherApiData to Forecast using Linq.
            //Your code

            var forecast = new Forecast()
            {
                City = wd.city.name,
                Items = wd.list.Select(x => new ForecastItem
                {
                    DateTime = UnixTimeStampToDateTime(x.dt),
                    Temperature = x.main.temp,
                    WindSpeed = x.wind.speed,
                    Description = x.weather[0].description
                }).ToList(),
            };
            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
