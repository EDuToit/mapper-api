using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mapper_Api.Context;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Mapper_Api.Services
{
    public partial class CommunicationService
    {
        WeatherService WeatherService;
        CourseDb CourseDb;
        public CommunicationService(WeatherService WeatherService, CourseDb CourseDB)
        {
            this.CourseDb = CourseDB;
            this.WeatherService = WeatherService;
        }

        public async Task SocketHandler(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                byte[] data = new byte[result.Count];
                Array.Copy(buffer, data, result.Count);
                var response = await GenerateResponse(data);
                await webSocket.SendAsync(response, result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task<ArraySegment<byte>> GenerateResponse(byte[] input)
        {
            string query = Encoding.ASCII.GetString(input);
            var result = JsonConvert.SerializeObject(await interpretInput(query));
            return new ArraySegment<Byte>(Encoding.ASCII.GetBytes(result.ToString()));
        }

        private async Task<ReturnMessage> interpretInput(string query)
        {
            try
            {
                var inputData = JsonConvert.DeserializeObject<LiveLocationMessage>(query);
                LiveUser liveUser = null;

                if ((liveUser = CourseDb.LiveUser
                        .Where(c => c.UserID == inputData.UserID).SingleOrDefault()) == null)
                {
                    liveUser = new LiveUser()
                    {
                        UserID = Guid.NewGuid()
                    };
                    CourseDb.LiveUser.Add(liveUser);
                }
                if (inputData.Location != null)
                {
                    CourseDb.LiveLocation.Add(new LiveLocation
                    {
                        UserID = liveUser.UserID,
                        GeoJson = inputData.Location
                    });
                }
                await CourseDb.SaveChangesAsync();
             
                return new ReturnMessage(){
                        Weather = "User string", 
                        UserID = liveUser.UserID
                    };
            }
            catch (Exception e)
            {
             throw new ArgumentException("Invalid user id or location");
            }
        }
    }
}