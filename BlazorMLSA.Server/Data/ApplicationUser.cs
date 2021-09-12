using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string PictureUri { get; set; }
        public bool IsOnline { get; set; }
        public int TabsOpen { get; set; }
        public List<Message> SentMessages { get; set; }
        public List<Message> ReceivedMessages { get; set; }
    }
}
