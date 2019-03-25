using System;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;

namespace RocketChatBot.API
{
    class RocketChatBot
    {
        public string _username { get; set; }
        public string _password { get; set; }
        public string _server { get; set; }
        public string _authToken { get; set; }
        private string _name { get; set; }
        private string _id { get; set; }
        private WebHeaderCollection _authHeaders;

        public RocketChatBot()
        {
            Authenticate();
        }

        public void StartListener(string channelId)
        {
            MessageListener msgL = new MessageListener(_server.Replace("https://", "").Replace("http://", "") + ":3000", _authToken, channelId);
            msgL.Start();
        }

        public RocketChatBot(string username, string password, string serverUrl)
        {
            _username = username;
            _password = password;
            _server = serverUrl;

            Authenticate();
        }

        private void Authenticate()
        {
            if (_username != string.Empty && _password != string.Empty && _server != string.Empty)
            {
                dynamic res = APICall.JSONPostRequest(_server + "/api/v1/login", "{\"user\": \"" + _username + "\", \"password\": \"" + _password + "\"}");
                if (res.status == "success")
                {
                    _authToken = res.data.authToken;
                    _name = res.data.me.name;
                    _id = res.data.me._id;
                    _authHeaders = new WebHeaderCollection();
                    _authHeaders["X-Auth-Token"] = _authToken;
                    _authHeaders["X-User-Id"] = _id;
                    Console.WriteLine("[SUCCESS] Authenticated as " + _name + " [" + _id + "]");
                }
                else
                {
                    Console.WriteLine("[ERROR] Authentication error. Are you using the correct login details?");
                }
            }
        }

        public void ListChannels()
        {
            if (_authHeaders != null)
            {
                dynamic channelsList = APICall.AuthenticatedGETRequest(_server + "/api/v1/channels.list", _authHeaders);
                var channelList = channelsList.channels;

                foreach(var channel in channelList)
                {
                    // Handle channel objects
                    // See https://rocket.chat/docs/developer-guides/rest-api/channels/list/
                }
            }
        }

        public void JoinChannel(string channelName)
        {
            string postdata = "{\"roomId\": \"" + channelName + "\"}";
            dynamic res = APICall.JSONPostRequest(_server + "/api/v1/channels.join", postdata, _authHeaders);
        }

        public void SendMessage(string channelId, string message)
        {
            string postData = "{ \"channel\": \"" + channelId + "\", \"text\": \"" + message + "\" }";
            dynamic res = APICall.JSONPostRequest(_server + "/api/v1/chat.postMessage", postData, _authHeaders);

        }
    }
}
