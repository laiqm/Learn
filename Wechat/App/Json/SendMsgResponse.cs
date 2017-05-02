using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.App.Json
{
    public class SendMsgResponse
    {
        public BaseResponse BaseResponse { get; set; }
        public long? MsgID { get; set; }
        public string LocalID { get; set; }
    }
}
