using PureWebSockets;
using System.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace RocketChatBot.API
{
    class MessageListener
    {
        private const string COMMAND_IDENTIFIER = "!";
        public string _auth { get; set; }
        public string _channelId { get; set; }
        private PureWebSocket ws;
        private Timer _timer;
        private int _sendCount;
        private string _url;

        public MessageListener()
        {

        }

        public MessageListener(string socketUrl, string authToken, string channelId)
        {
            _auth = authToken;
            _channelId = channelId;
            _url = socketUrl;
        }

        public async void Start()
        {
            var socketOptions = new PureWebSocketOptions()
            {
                DebugMode = false,
                SendDelay = 100,
                IgnoreCertErrors = true,
                MyReconnectStrategy = new ReconnectStrategy(2000, 4000, 20)
            };
            ws = new PureWebSocket("ws://" + _url + "/websocket", socketOptions);
            ws.OnStateChanged += Ws_OnStateChanged;
            ws.OnMessage += Ws_OnMessage;
            ws.OnClosed += Ws_OnClosed;
            ws.OnSendFailed += Ws_OnSendFailed;
            await ws.ConnectAsync();

            Console.WriteLine("SENDING WEBSOCKET MESSAGE");
            string[] s = new string[1];
            s[0] = "1";
            WebSocket.ConnectMsg c = new WebSocket.ConnectMsg()
            {
                msg = "connect",
                version = "1",
                support = s
            };

            await ws.SendAsync(JsonConvert.SerializeObject(c));

            Console.ReadLine();
            ws.Dispose(true);

            await ws.ConnectAsync();
        }

        private void Ws_OnClosed(WebSocketCloseStatus reason)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} Connection Closed: {reason}");
            Console.ResetColor();
            Console.WriteLine("");
            Console.ReadLine();
        }

        private void Ws_OnSendFailed(string data, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} Send Failed: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("");
        }

        private async void OnTickAsync(object state)
        {
            if (ws.State != WebSocketState.Open) return;

            if (_sendCount == 1000)
            {
                _timer = new Timer(OnTickAsync, null, 30000, 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now} Max Send Count Reached: {_sendCount}");
                Console.ResetColor();
                Console.WriteLine("");
                _sendCount = 0;
            }
            if (await ws.SendAsync(_sendCount + " | " + DateTime.Now.Ticks.ToString()))
            {
                _sendCount++;
            }
            else
            {
                Ws_OnSendFailed("", new Exception("Send Returned False"));
            }
        }

        private void Ws_OnMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now} New message: {message}");
            Console.ResetColor();
            Console.WriteLine("");
            dynamic msg = JsonConvert.DeserializeObject(message);
            if (msg.msg == "ping")
            {
                WebSocket.Message wsMessage = new WebSocket.Message()
                {
                    msg = "pong"
                };
                ws.SendAsync(JsonConvert.SerializeObject(msg));
            }

            if (msg.msg == "connected")
            {
                // Go into auth stage
                ws.SendAsync("{\"msg\": \"method\",\"method\": \"login\",\"id\": \"GENERAL\",\"params\": [{\"resume\": \"" + _auth + "\" }]}");
            }

            if (msg.msg == "updated")
            {
                ws.SendAsync("{\"msg\": \"sub\",\"id\": \"GENERAL\",\"name\": \"stream-room-messages\",\"params\":[\"GENERAL\",false]}");
            }

            if (msg.msg == "changed")
            {
                if (msg.fields.args[0].msg.ToString().Substring(0, 1) == COMMAND_IDENTIFIER)
                {
                    new MessageHandler(msg.fields.args[0].msg.ToString(), _channelId);
                }
            }
        }

        private void Ws_OnStateChanged(WebSocketState newState, WebSocketState prevState)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now} Status changed from {prevState} to {newState}");
            Console.ResetColor();
            Console.WriteLine("");
        }
    }
}
