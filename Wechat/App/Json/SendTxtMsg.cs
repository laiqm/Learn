using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.App.Json
{
    public class SendTxtMsg
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public string ClientMsgId { get; set; }
        public string LocalID { get; set; }
    }
}
