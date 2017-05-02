using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Wechat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Test()
        {
            var sb = new StringBuilder();

            var now = DateTime.Now.ToUniversalTime();
            var start = new DateTime(1970, 1, 1, 0, 0, 0, now.Kind);
            var s3 = Convert.ToInt64((now - start).TotalMilliseconds);
            sb.Append("s3 " + s3);

            sb.AppendLine("<br/>s4 " + (uint)(s3 << 32 - Int32.MaxValue));
            var random = new Random();
            sb.AppendLine("<br/>"+ random.Next(0,9999));
            sb.AppendLine("<br/>" + random.Next(0, 9999));
            //TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Content(sb.ToString());
        }
    }
}
