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

            List<TaiKhoan> listTK = db.TaiKhoans.Where(o => o.DelTime == null).ToList();

            return View(listTK);
        }

        public ActionResult ThemTaiKhoan()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new TaiKhoan());
        }

        public Admin GetAdmin(string maad)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var ad = db.Admins.Where(o => o.DelTime == null).SingleOrDefault();

                if (!string.IsNullOrEmpty(maad))
                {
                    ad = db.Admins.Where(o => o.MaAdmin == maad && o.DelTime == null).SingleOrDefault();
                }

                return ad;
            }

            return null;
        }
    }
}