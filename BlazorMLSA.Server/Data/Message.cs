using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Data
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
