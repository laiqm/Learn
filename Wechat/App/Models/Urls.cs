using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wechat.App.Models
{
    public class Urls
    {
        public static string SessionUrl => "https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=";

        public static string QRcodeUrl => "https://login.weixin.qq.com/qrcode/";

        public static string LoginCheckUrl => "https://login.wx2.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip={1}&r={2}&r={2}&_={3}";
        //https://login.wx2.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=4aOah5KUDA==&tip=0&r=1053562852&_=1493595005460
        //public static string LoginCheckUrl => "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}";

        public static string InitUrl => "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={0}&lang=zh_CN&pass_ticket={1}";

        //https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?lang=zh_CN&pass_ticket=q9X4H1zNLHLGOsETIoHTKMpzzcrHA%252FDdou1xySUFciAQq9VuZShpcDiBjtqeih3T&r=1493634404148&seq=0&skey=@crypt_8f808217_6145a3ed0bb12fb60f16cc15d975eba1
        public static string ContactUrl => "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?lang_zh_CN&pass_ticket={0}&r={1}&seq=0&skey={2}";

        public static string SyncCheckUrl => "https://webpush.wx2.qq.com/cgi-bin/mmwebwx-bin/synccheck";

        public static string SendMsgUrl => "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid=";

        public static string SyncUrl => "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={0}&lang=zh_CN&skey={1}&pass_ticket={2}";

        public static string RemarkUrl => "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxoplog?pass_ticket=";
    }
}
