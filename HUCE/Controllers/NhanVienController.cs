using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class NhanVienController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: NhanVien
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

        public NhanVien GetNhanVien(string manv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.NhanViens.Where(o => o.MaNV == manv && o.DelTime == null).SingleOrDefault(); ;
            }

            return null;
        }

        public ActionResult ChiTietNhanVien(string manv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var nv = db.NhanViens.Where(o => o.MaNV == manv && o.DelTime == null).SingleOrDefault();

            return View(nv);
        }
    }
}