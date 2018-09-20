using System;

namespace Mapper_Api.Services
{
    public partial class CommunicationService
    {
        public class ReturnMessage {
            public String Weather { get; set; }
            public Guid UserID { get; set; }
        }
    }
}