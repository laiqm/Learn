using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.App.Json
{
    public class BaseRequest
    {
        public string Uin { get; set; }
        public string Sid { get; set; }
        public string Skey { get; set; }
        public string DeviceID { get; set; }
    }
}
