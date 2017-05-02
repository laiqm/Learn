using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wechat.Entities
{
    public class CleanTask
    {
        //sessionId
        public string Id { get; set; }

        public string UserDisplay { get; set; }

        public string Avatar { get; set; }

        public string UserDetail { get; set; }

        public string ContactDetail { get; set; }

        public int FriendCount { get; set; }

        public int GroupCount { get; set; }

        public int PublicCount { get; set; }

        public int ZombieCount { get; set; }

        public string ZombieList { get; set; }

        public DateTime CreateTime { get; set; }

        public Account Creator { get; set; }

        public CleanStatus Status { get; set; }

        public string Content { get; set; }

        public int SendMsgCount { get; set; }
    }
}
