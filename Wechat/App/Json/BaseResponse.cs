using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.App.Json
{
    /// <summary>
    /// 接收消息必须带的部分
    /// </summary>
    public class BaseResponse
    {
        public int Ret { get; set; }
        public string ErrMsg { get; set; }
    }
}
