using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.App.Json
{
    public class WechatGetContact
    {
        public BaseResponse BaseResponse { get; set; }
        public int MemberCount { get; set; }
        public User[] MemberList { get; set; }
    }
}
