using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class HocBongController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: HocBong
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachHocBong()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }
    }
}