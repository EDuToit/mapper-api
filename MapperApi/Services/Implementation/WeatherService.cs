/***
 * Filename: ElementsController.cs
 * Author  : Eben du Toit
 * Class   : UserService
 *
 *      Service for weather information Implementation not used in app but working
 ***/

using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Mapper_Api.Services
{

    public class WeatherService : IWeatherService
    {
        string AppKey;
        public WeatherService()
        {
            string appKey = "643fa9db96b5c946db296ff59f39ed50";
            this.AppKey = appKey;
        }

        public async Task<string> GetWeatherInLatLng(double lat, double lng)
        {
            string baseUrl = $"http://api.openweathermap.org/data/2.5/weather?lat={lat.ToString()}&lon={lng.ToString()}&appid={this.AppKey}";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    return data;
                }
                return "";
            }
        }
    }
}
