using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class DiemRenLuyenController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: DiemRenLuyen
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachDiemRenLuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<SinhVien> listsv = db.SinhViens.Where(o => o.DelTime == null).ToList();
            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            return View(listsv);
        }

        public ActionResult DanhGiaDiemRenLuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }

        [HttpPost]
        public ActionResult DanhGiaDiemRenluyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var masv = SessionConfig.GetSession();

            return View();
        }
    }
}