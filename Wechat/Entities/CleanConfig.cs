using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wechat.Entities
{
    public class CleanConfig
    {
        public int Id { get; set; }

        public string MessagePrefix { get; set; }

        public string MessageSuffix { get; set; }

        public int CheckLoginRetryCount { get; set; }

        public int SendMsgInteval { get; set; }

        public int SendMsgFailRetryCount { get; set; }

        public int SendMsgFailRetryInteval { get; set; }
    }
}
