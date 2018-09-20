using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Mapper_Api.Services
{

    public class WeatherService
    {
        string AppKey;
        public WeatherService()
        {
            string appKey = "643fa9db96b5c946db296ff59f39ed50";
            this.AppKey = appKey;
        }

        public async Task<WeatherResult> GetWeatherInLatLng(double Lat, double Lng)
        {
            string baseUrl = $"http://api.openweathermap.org/data/2.5/weather?lat={Lat.ToString()}&lon={Lng.ToString()}&appid={this.AppKey}";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    return (JsonConvert.DeserializeObject<WeatherResult>(data));
                }
                return null;
            }
        }
    }
}
