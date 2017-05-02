using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wechat.App.Json;
using Wechat.App.Models;
using Wechat.Entities;
using Wechat.Utils;

namespace Wechat.App
{
    /// <summary>
    /// 注意线程不安全
    /// </summary>
    public class CleanService
    {
        private readonly ILogger log;
        private readonly IConfigurationRoot config;

        public CleanService(IConfigurationRoot config, ILoggerFactory factory)
        {
            log = factory.CreateLogger("app");
            this.config = config;
            context = new CleanContext();
        }

        public static CleanTask CreateTask(HttpProxy proxy, Account account, WechatContext context)
        {
            var now = DateTime.Now;
            var url = Urls.SessionUrl + TimeTic(now);
            var result = proxy.Get(url);
            var sessionId = result.Split(new[] { "\"" }, StringSplitOptions.None)[1];
            var task = new CleanTask
            {
                Id = sessionId,
                Creator = account,
                CreateTime = now
            };
            context.Add(task);
            context.SaveChanges();
            return task;
        }

        private static string RandomNum()
        {
            var charArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var sCode = "";
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 15; i++)
            {
                sCode += charArray[random.Next(charArray.Length)];
            }
            return sCode;
        }

        private int Bit64Not(long num)
        {
            var str = Convert.ToString(num, 2);
            var start = str.Length >= 32 ? (str.Length - 32) : 0;
            var length = str.Length >= 32 ? 32 : str.Length;
            var bit32 = str.Substring(start, length);
            return ~Convert.ToInt32(bit32, 2);
        }

        private static long TimeTic(DateTime time)
        {
            var now = time.ToUniversalTime();
            var start = new DateTime(1970, 1, 1, 0, 0, 0, now.Kind);
            return (long)(now - start).TotalMilliseconds;
        }

        private static long TimeTick
        {
            get
            {
                var now = DateTime.Now.ToUniversalTime();
                var start = new DateTime(1970, 1, 1, 0, 0, 0, now.Kind);
                return (long)(now - start).TotalMilliseconds;
            }
        }

        #region for clean

        private readonly CleanContext context;
        private CleanTask task;

        private HttpProxy _http;
        private WechatContext _db;
        private CleanArg _arg;
        private long _tick;
        private void init()
        {
            var builder = new DbContextOptionsBuilder<WechatContext>();
            builder.UseSqlServer(config.GetConnectionString("WechatContext"));
            _db = new WechatContext(builder.Options);

            _http = new HttpProxy();
        }

        [AutomaticRetry(Attempts = 0)]
        public void Clean(string sessionId, CleanArg arg = null)
        {
            _arg = arg ?? new CleanArg();
            init();
            try
            {
                log.LogDebug("开始进行清理任务。sessionId: " + sessionId);
                task = _db.Task.SingleOrDefault(x => x.Id == sessionId);
                _tick = TimeTic(task.CreateTime);
                if (task == null)
                {
                    log.LogDebug("库中未找到清理任务，任务结束。sessionId: " + sessionId);
                    return;
                }
                task.Content = (_arg.Content ?? Config.MessagePrefix) + "\r\n\r\n\r\n" + (_arg.Suffix ?? Config.MessageSuffix);
                CheckLogin();
                if (task.Status != CleanStatus.Logined)
                {
                    debug("长时间未扫码，取消本次任务");
                    return;
                }

                InitLoginInfo();
                InitLoginUser();
                SyncContact();
                SyncCheck();
                SendMsg();
                while (!_stopSync) { } //等待最后一次同步完成
                Complete();
                //SyncMsg();
            }
            catch (Exception ex)
            {
                task.Status = CleanStatus.Error;
                update();
                log.LogError(1, ex, "清理过程中发生错误, sessionId: " + task.Id);
                throw;
            }
            finally
            {
                _stopSync = true;
                _http.Dispose();
                _db.Dispose();
            }
        }
        private void debug(string msg, params object[] args)
        {
            var id = task == null ? null : task.Id;
            log.LogDebug(msg + "。sessionId: " + id, args);
        }

        private void update()
        {
            _db.Update(task);
            _db.SaveChanges();
        }

        private void CheckLogin()
        {
            debug("开始检测登录");

            for (var count = 0; count < Config.CheckLoginRetryCount; count++) //尝试20次，如果未登录则直接取消这次登录
            {
                var url = string.Format(Urls.LoginCheckUrl, task.Id, count == 0 ? 1 : 0, Bit64Not(TimeTick), ++_tick);
                debug("检测登录尝试第{0}次", count);

                var result = _http.Get(url); // 未登录的情况下25秒左右返回
                debug("登录返回信息:{0}", result);
                if (result.Contains("=201"))//已扫描，未登录
                {
                    var base64 = result.Split(new[] { "\'" }, StringSplitOptions.None)[1].Split(',')[1];
                    task.Status = CleanStatus.Scaned;
                    task.Avatar = base64;
                    update();

                    debug("检测登录第{0}次发现二维码被扫描", count);
                }
                else if (result.Contains("=200"))//已扫描 已登录
                {
                    debug("检测登录第{0}次登录成功", count);

                    var redirectUrl = result.Split(new[] { "\"" }, StringSplitOptions.None)[1];
                    context.RedirectUrl = redirectUrl;
                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        debug("登录后获取到的跳转页面为空");
                        task.Status = CleanStatus.Error;
                    }
                    else
                    {
                        task.Status = CleanStatus.Logined;
                    }
                    update();
                    return;
                }
                else if (!result.Contains("=408"))
                { //400为二维码已经过期
                    debug("登录检测返回消息不正确，取消本次任务");
                    break;
                }
            }
            task.Status = CleanStatus.Canceled;
            update();
            debug("检测登录结束，未登录");
        }

        private void InitLoginInfo()
        {
            debug("开始初始化登录信息");
            //返回值：https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxnewloginpage?ticket=A3bY6tLyf-i3j0f6ZLg9-A3M@qrticket_0&uuid=oZCSv7d0uA==&lang=zh_CN&scan=1493634399
            //实际请求：https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxnewloginpage?ticket=A3bY6tLyf-i3j0f6ZLg9-A3M@qrticket_0&uuid=oZCSv7d0uA==&lang=zh_CN&scan=1493634399&fun=new&version=v2&lang=zh_CN
            var url = new Uri(context.RedirectUrl);
            var response = _http.Get(context.RedirectUrl + "&fun=new&version=v2&lang=zh_CN");

            var xmlDoc = XDocument.Parse(response);
            var node = xmlDoc.Root;
            if (!node.Element("ret").Value.Equals("0"))
            {
                var msg = "初始化登录信息失败, sessionId: " + task.Id;
                log.LogError(msg);
                throw new Exception(msg);
            }
            context.PassTicket = node.Element("pass_ticket").Value;

            context.BaseRequest = new BaseRequest
            {
                Sid = node.Element("wxsid").Value,
                Uin = node.Element("wxuin").Value,
                Skey = node.Element("skey").Value,
                DeviceID = "e" + RandomNum()
            };
            debug("初始化登录信息结束");
        }

        private void InitLoginUser()
        {   //最好重试一次，网页中会cancel一次,在这之后会有一个post
            //https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxstatusnotify?lang=zh_CN&pass_ticket=q9X4H1zNLHLGOsETIoHTKMpzzcrHA%252FDdou1xySUFciAQq9VuZShpcDiBjtqeih3T
            //返回结果示例{"BaseResponse": {"Ret": 0,"ErrMsg": ""},"MsgID": "7436838215428686364"}

            debug("开始初始化登录用户");
            var url = string.Format(Urls.InitUrl, Bit64Not(TimeTick), context.PassTicket);
            var body = JsonHelper.Serialize(new { context.BaseRequest });
            var resposne = _http.Post(url, body);
            var init = JsonHelper.DeSerialize<WechatInit>(resposne);
            if (init.BaseResponse.Ret != 0)
            {
                var msg = "初始化登录用户失败，sessionId: " + task.Id;
                log.LogError(msg);
                throw new Exception(msg);
            }
            task.UserDisplay = init.User.DisplayName;
            task.Status = CleanStatus.Inited;
            task.UserDetail = JsonHelper.Serialize(init.User);
            update();

            context.LoginUser = init.User;
            context.SyncKey = init.SyncKey;

            debug("初始化登录用户结束，登录用户显示名称: {0}", init.User.DisplayName);
        }

        private void SyncContact()
        {
            debug("开始获取联系人信息");
            var url = string.Format(Urls.ContactUrl, context.PassTicket, TimeTick, context.BaseRequest.Skey);
            //之后还有一个请求 https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=1493634404279&lang=zh_CN&pass_ticket=q9X4H1zNLHLGOsETIoHTKMpzzcrHA%252FDdou1xySUFciAQq9VuZShpcDiBjtqeih3T
            /*
             * {BaseRequest:{DeviceID:"e079715916286170",Sid:"9vhqLiOafLtiGjMk",Skey:"@crypt_8f808217_6145a3ed0bb12fb60f16cc15d975eba1",Uin:2043832471,Count:4,List:[
             * {UserName: "@@def09193834e88ba787981d9834c6990e2e68b32610e020dbeed8254d602ed36", EncryChatRoomId: ""}
             * {UserName: "@@d93e92eba5d1c0b5908b24f067b2452a984bbf07e3e6356aa20525a1aca33c3e", ChatRoomId: ""}
             * ]
             */
            var response = _http.Get(url);
            var contactList = JsonHelper.DeSerialize<WechatGetContact>(response);

            context.Friends = contactList.MemberList.Where(x => !x.IsGroup && !x.IsPublic).ToList();
            context.Groups = contactList.MemberList.Where(x => x.IsGroup).ToList();

            task.FriendCount = context.Friends.Count;
            task.GroupCount = context.Groups.Count;
            task.PublicCount = contactList.MemberCount - context.Friends.Count - context.Groups.Count;
            task.ContactDetail = JsonHelper.Serialize(contactList.MemberList);
            debug("获取联系人信息结束，联系人总数:{0},朋友数量:{1}，群数量:{2}",
                contactList.MemberCount, task.FriendCount, task.GroupCount);

        }

        private void SendMsg()
        {
            debug("开始群发消息,消息内容：{0}", task.Content);

            var filter = (_arg.Filter ?? "")
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var tryCount = 0;
            var index = _arg.Skip;
            for (; index < context.Friends.Count; index++)
            {
                if (_stopSync)
                {
                    debug("已经停止同步，不再继续发消息");
                    throw new Exception("已经停止同步，不再继续发消息");
                }
                var friend = context.Friends[index];
                if (friend.DisplayName != null && filter.Any(x => friend.DisplayName.Contains(x)))
                {
                    debug("用户“{0}”");
                    continue;
                }
                try
                {
                    SendMsg(friend, task.Content);
                    Thread.Sleep(Config.SendMsgInteval);
                }
                catch
                {
                    log.LogError("发送消息失败，等待10秒继续,friendName:{0},sessionId:{1},重试次数:{2}",
                        friend.DisplayName, task.Id, Config.SendMsgFailRetryCount - tryCount + 1);
                    tryCount++;
                    if (tryCount > Config.SendMsgFailRetryCount)
                        break;
                    Thread.Sleep(Config.SendMsgFailRetryInteval);
                }
            }
            debug("群发消息结束，发送消息条数:{0}", index - _arg.Skip);
            task.SendMsgCount = index - _arg.Skip;
            task.Status = CleanStatus.Backcasted;
            update();

        }

        private Random random = new Random();
        private void SendMsg(User user, string content)
        {
            debug("开始发消息给:{0}", user.DisplayName);
            var url = string.Format(Urls.SendMsgUrl, context.BaseRequest.Sid, context.PassTicket);
            var id = TimeTick + (random.Next(0, 9999).ToString().PadLeft(4, '0'));
            var body = new
            {
                context.BaseRequest,
                Msg = new SendTxtMsg
                {
                    FromUserName = context.LoginUser.UserName,
                    ToUserName = user.UserName,
                    Type = 1,
                    Content = content,
                    ClientMsgId = id,
                    LocalID = id
                }
            };
            var bodyJson = JsonHelper.Serialize(body);
            var response = _http.Post(url, bodyJson);
            var result = JsonHelper.DeSerialize<SendMsgResponse>(response);
            debug("结束发消息给:{0}，消息返回结果：{1}", user.DisplayName, response);
            if (result.BaseResponse.Ret != 0)
            {
                var msg = string.Format("发送消息失败，接收方:{0}, sessionId:{1}, result:{2}",
                    user.DisplayName, task.Id, response);
                log.LogError(msg);
                throw new Exception(msg);
            }
        }

        private bool _stopSync = false;
        private void SyncCheck()
        {
            //https://webpush.wx2.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1493634404152&skey=%40crypt_8f808217_6145a3ed0bb12fb60f16cc15d975eba1&sid=9vhqLiOafLtiGjMk&uin=2043832471&deviceid=e112474750446635&synckey=1_662103592%7C2_662103640%7C3_662103643%7C1000_1493632441&_=1493631186270
            var baseUrl = Urls.SyncCheckUrl +
                "?r=" + TimeTick +
                "&skey=" + context.BaseRequest.Skey +
                "&sid=" + context.BaseRequest.Sid +
                "&uin=" + context.BaseRequest.Uin +
                "&deviceid=" + context.BaseRequest.DeviceID;
            debug("开始同步检查");
            Task.Run(() =>
            {
                var count = 1;
                while (!_stopSync)
                {
                    var url = baseUrl + "&synckey=" + context.SyncKey.get_urlstring() + "&_=" + (++_tick); //tick接着logincheck
                    var response = _http.Get(url);//没有消息的情况下25秒左右返回
                    var selector = Regex.Match(response, "(?<=.*selector:\")\\d+(?=.*)").Groups[0].Value;
                    var retcode = Regex.Match(response, "(?<=.*retcode:\")\\d+(?=.*)").Groups[0].Value;
                    debug("同步第{0}次,同步结果:{1},请求地址：{2}", count++, response, url);
                    if (retcode != "0")//同步失败或者手动结束
                    {
                        log.LogError("同步失败,同步结束");
                        _stopSync = true;
                        break;
                    }
                    if (selector != "0")//有新消息
                    {
                        debug("发现新消息");
                        SyncMsg();
                    }
                    if (task.Status >= CleanStatus.Backcasted)
                    {
                        _stopSync = true;
                    }
                    Thread.Sleep(2000);
                }
            });
        }

        private void SyncMsg()
        {
            debug("开始同步消息");
            var url = string.Format(Urls.SyncUrl,
                context.BaseRequest.Sid,
                context.BaseRequest.Skey,
                context.PassTicket);
            var body = JsonHelper.Serialize(new
            {
                context.BaseRequest,
                context.SyncKey,
                rr = Bit64Not(TimeTick)
            });

            var response = _http.Post(url, body);
            var sync = JsonHelper.DeSerialize<WechatSync>(response);
            context.SyncKey = sync.SyncKey;

            if (sync.BaseResponse.Ret != 0 || sync.AddMsgCount == 0)
            {
                var msg = "同步结束，没收到消息, sessonId: " + task.Id;
                log.LogWarning(msg);
                return;
            }
            debug("同步完成");
            Remark(sync);

            //https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxstatreport?fun=new&lang=zh_CN&pass_ticket=q9X4H1zNLHLGOsETIoHTKMpzzcrHA%252FDdou1xySUFciAQq9VuZShpcDiBjtqeih3T
            /* post
             * {BaseRequest:{},Count:1,List:[{Type:1,Text:{"{"type":"[app-timing]","data":{"appTiming":{"qrcodeStart":1493634381246,"qrcodeEnd":1493634381594,"scan":1493634397415,"loginEnd":1493634403504,"initStart":1493634403506,"initEnd":1493634404084,"initContactStart":1493634404148},"pageTiming":{"navigationStart":1493631185627,"unloadEventStart":0,"unloadEventEnd":0,"redirectStart":0,"redirectEnd":0,"fetchStart":1493631185746,"domainLookupStart":1493631185746,"domainLookupEnd":1493631185746,"connectStart":1493631185746,"connectEnd":1493631185746,"secureConnectionStart":0,"requestStart":1493631185748,"responseStart":1493631185852,"responseEnd":1493631185987,"domLoading":1493631185873,"domInteractive":1493631186435,"domContentLoadedEventStart":1493631186435,"domContentLoadedEventEnd":1493631186437,"domComplete":1493631186557,"loadEventStart":1493631186557,"loadEventEnd":1493631186559}}}"}}]}
             */
        }

        private void Remark(WechatSync sync)
        {
            var mark = "A00僵尸=";
            debug("开始检测僵尸");
            foreach (var msg in sync.AddMsgList)
            {
                if (msg.MsgType == 10000)
                {
                    if (msg.Content.Contains("开启了朋友验证") || msg.Content.Contains("但被对方拒收"))
                    {
                        var zombie = context.Friends.Find(x => x.UserName == msg.FromUserName);
                        //list.Add(zombie);
                        debug("检测出僵尸,开始标记:{0}", zombie.DisplayName);
                        if (!zombie.DisplayName.StartsWith(mark))
                            Remark(zombie, mark + zombie.DisplayName);
                        zombies.Add(zombie);
                        debug("标记完成:{0}", zombie.DisplayName);
                        Thread.Sleep(Config.SendMsgInteval);
                    }
                }
            }
        }

        private void Remark(User user, string mark)
        {
            var url = Urls.RemarkUrl + context.PassTicket;
            var body = new
            {
                context.BaseRequest,
                CmdId = 2,
                RemarkName = mark,
                UserName = user.UserName
            };
            var bodyJson = JsonHelper.Serialize(body);
            var response = _http.Post(url, bodyJson);
        }

        private List<User> zombies = new List<User>();
        private void Complete()
        {

            debug("检测僵尸结束，共检测出{0}个僵尸,正在保存战果", zombies.Count);
            task.ZombieCount = zombies.Count;
            task.ZombieList = JsonHelper.Serialize(zombies);
            task.Status = CleanStatus.Completed;
            update();
        }

        #endregion
    }
}
