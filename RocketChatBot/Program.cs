using System;

namespace RocketChatBot
{
    class Program
    {
        public static API.RocketChatBot RCB { get; set; }
        private const string USERNAME = "bot";
        private const string PASSWORD = "";
        private const string SERVER = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to chat");
            RCB = new API.RocketChatBot(USERNAME, PASSWORD, SERVER);
            // RCB.ListChannels();
            // RCB.JoinChannel("GENERAL");

            // Start incoming message listener
            RCB.StartListener("GENERAL");
            RCB.SendMessage("GENERAL", "Init.");
            Console.ReadLine();
        }
    }
}
