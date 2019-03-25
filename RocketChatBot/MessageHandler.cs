namespace RocketChatBot
{
    class MessageHandler
    {
        private API.RocketChatBot RCB = Program.RCB;
        public MessageHandler(string message, string channel)
        {
            // Handle commands here
            if (message == "!hi")
                RCB.SendMessage(channel, "Hello, world!");
        }
    }
}
