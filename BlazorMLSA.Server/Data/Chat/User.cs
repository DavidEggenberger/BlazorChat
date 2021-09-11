using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Data.Chat
{
    public class User
    {
        public Guid Id { get; set; }
        public bool IsOnline { get; set; }
        public int TabsOpen { get; set; }
        public List<Message> SentMessages { get; set; }
        public List<Message> ReceivedMessages { get; set; }
    }
}
