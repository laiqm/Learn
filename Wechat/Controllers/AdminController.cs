using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wechat.Entities;

namespace Wechat.Controllers
{
    public class AdminController : Controller
    {
        private readonly WechatContext _db;
        public AdminController(WechatContext db) {
            _db = db;
        }
        public IActionResult Index()
        {
            return Content("û���������Ŷ");
        }

        public IActionResult Config() {
            var config = _db.Config.FirstOrDefault();
            return View(config);
        }

        [HttpPost]
        public IActionResult Config(CleanConfig config) {

            config.Id = 1;
            _db.Update(config);
            _db.SaveChanges();

            return RedirectToAction("Action");
        }
    }
}