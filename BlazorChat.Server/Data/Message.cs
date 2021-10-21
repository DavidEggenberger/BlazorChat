using System;

namespace BlazorChat.Server.Data
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }
    }
}
