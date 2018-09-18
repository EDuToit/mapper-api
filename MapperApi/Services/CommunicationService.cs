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
    public class CommunicationService
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
            var result = JsonConvert.SerializeObject(interpretInput(query));
            return new ArraySegment<Byte>(Encoding.ASCII.GetBytes(result.ToString()));
        }

        private async Task<List<LiveLocationMessage>> interpretInput(string query)
        {
            var newList = new List<LiveLocationMessage>();
            try
            {
                var inputData = JsonConvert.DeserializeObject<LiveLocationMessage>(query);
                LiveUser liveUser = null;
                if (inputData.UserID != null)
                {
                    liveUser = CourseDb.LiveUser.Where(c => c.UserID == inputData.UserID).SingleOrDefault();
                }

                if (liveUser == null)
                {
                    liveUser = new LiveUser
                    {
                        UserID = new Guid()
                    };

                    CourseDb.LiveUser.Add(liveUser);
                }
                string additionalInfo = "";

                if (inputData.Location != null){
                    CourseDb.LiveLocation.Add(new LiveLocation {
                        UserID = liveUser.UserID, 
                        GeoJson = inputData.Location
                    });
                    additionalInfo += inputData.Location;
                }

                await CourseDb.SaveChangesAsync();

                if (inputData.UserID != null)
                {
                    newList.Add(new LiveLocationMessage()
                    {
                        UserID = liveUser.UserID,
                        Device = inputData.Device,
                        Location = @"User Sent us info already yay " + additionalInfo
                    });
                }
                else
                {
                    if (inputData.Device == LiveLocationMessage.DeviceType.MONITOR)
                    {
                        newList.Add(new LiveLocationMessage()
                        {
                            UserID = liveUser.UserID,
                            Device = inputData.Device,
                            Location = " User Created as MONITOR "
                        });
                    }
                    if (inputData.Device == LiveLocationMessage.DeviceType.APPLICATION)
                    {
                        newList.Add(new LiveLocationMessage()
                        {
                            UserID = liveUser.UserID,
                            Device = inputData.Device,
                            Location = "User Created as APPLICATION"
                        });
                    }
                    if (inputData.Device == null && inputData.UserID == Guid.Empty)
                    {
                        newList.Add(new LiveLocationMessage()
                        {
                            UserID = liveUser.UserID,
                            Location = "User Created type unknown"
                        });
                    }
                }
            }

            catch (Exception e)
            {
                newList.Add(new LiveLocationMessage()
                {
                    // Description = "-- Its name is --",
                    // Payload = "Error"
                });
            }
            return newList;
        }
    }
}