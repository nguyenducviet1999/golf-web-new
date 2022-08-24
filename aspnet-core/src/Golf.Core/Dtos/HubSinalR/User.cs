using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.HubSinalR
{
    public class User
    {
        public string ConnectionId { get; set; }//Định danh kết nối WebSocket
        public Guid UserID { get; set; }//Định danh người dùng
    }
}
