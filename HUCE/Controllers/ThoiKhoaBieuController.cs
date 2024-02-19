using HUCE.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class ThoiKhoaBieuController : Controller
    {
        // GET: ThoiKhoaBieu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LichDay()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }
    }
}