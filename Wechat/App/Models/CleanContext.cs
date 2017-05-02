using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wechat.App.Json;

namespace Wechat.App.Models
{
    public class CleanContext
    {
        public string RedirectUrl { get; set; }

        public string PassTicket { get; set; }

        public BaseRequest BaseRequest { get; set; }

        public User LoginUser { get; set; }

        public SyncKey SyncKey { get; set; }
        
        public List<User> Friends { get; set; }

        public List<User> Groups { get; set; }
    }
}
