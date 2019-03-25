using System;
using System.Collections.Generic;
using System.Text;

namespace RocketChatBot.WebSocket
{
    class ConnectMsg
    {
        public string msg { get; set; }
        public string version { get; set; }
        public string[] support { get; set; }
    }
}
