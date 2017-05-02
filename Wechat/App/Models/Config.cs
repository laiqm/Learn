using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wechat.Entities;

namespace Wechat.App.Models
{
    public class Config
    {
        public static string MessagePrefix = "感谢您一直把我留在您的好友列表里，系统正在检测删除我的人，请勿回复。。  ";

        public static string MessageSuffix = "僵尸粉检测技术支持:\nhttp://www.kaolaproxy.cn/ \n打开链接，自助清理死粉";

        public static int CheckLoginRetryCount = 20;

        public static int SendMsgInteval = 2000;

        public static int SendMsgFailRetryCount = 5;

        public static int SendMsgFailRetryInteval = 10000;

        public static void Init(CleanConfig config)
        {
            if (config == null)
                return;
            MessagePrefix = config.MessagePrefix;
            MessageSuffix = config.MessageSuffix;
            CheckLoginRetryCount = config.CheckLoginRetryCount;
            SendMsgInteval = config.SendMsgInteval;
            SendMsgFailRetryInteval = config.SendMsgFailRetryInteval;
            SendMsgFailRetryCount = config.SendMsgFailRetryCount;
        }
    }
}
