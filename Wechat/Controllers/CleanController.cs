using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wechat.Entities;
using Wechat.App;
using Wechat.Utils;
using Microsoft.Extensions.Logging;
using Wechat.App.Models;

namespace Wechat.Controllers
{
    public class CleanController : Controller
    {
        private readonly WechatContext _context;
        private readonly HttpProxy _http;
        private readonly ILoggerFactory _logFactory;
        private readonly ILogger _log;

        public CleanController(WechatContext context, HttpProxy http, ILoggerFactory factory)
        {
            _context = context;
            _http = http;
            _logFactory = factory;
            _log = factory.CreateLogger("app");
        }

        // GET: Clean
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(CleanArg arg)
        {
            var admin = _context.Account.SingleOrDefault(x => x.Id == 1);

            var task = CleanService.CreateTask(_http, admin, _context);

            Hangfire.BackgroundJob.Schedule<CleanService>(x => x.Clean(task.Id, arg), TimeSpan.FromSeconds(10));
            
            return RedirectToAction("Check", new { task.Id });
        }

        public IActionResult Check(string id) {
            var task = _context.Task.SingleOrDefault(x => x.Id == id);
            if (task == null)
                return NotFound();
            if (task.Status >= CleanStatus.Logined) {
                return Content("你的微信正在清理中，请耐心等待");
            }
            var qr = Urls.QRcodeUrl + task.Id;

            return View((object)qr);
        }
    }
}
