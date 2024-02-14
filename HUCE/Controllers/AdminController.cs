using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class AdminController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }

        public Admin GetAdmin(string maad)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
               return db.Admins.Where(o => o.MaAdmin == maad && o.DelTime == null).SingleOrDefault();
            }

            return null;
        }

        public ActionResult ChiTietAdmin(string maad)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var admin = db.Admins.Where(o => o.MaAdmin == maad && o.DelTime == null).SingleOrDefault();

            return View(admin);
        }
    }
}