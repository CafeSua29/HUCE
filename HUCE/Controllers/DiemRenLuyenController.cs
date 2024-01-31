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

            return View(new DiemRenLuyen());
        }

        [HttpPost]
        public ActionResult DanhGiaDiemRenluyen(DiemRenLuyen drl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var masv = SessionConfig.GetSession();

                var dtn = DateTime.Now;

                var hocky = dtn.Month;
                var namhoc = dtn.Year;

                if(hocky <= 6)
                {
                    hocky = 1;
                }
                else
                {
                    hocky = 2;
                }

                var mahk = namhoc + "_" + hocky;

                var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                if (qr.Any())
                {
                    TempData["Error"] = "Ban da danh gia roi";
                    return View(drl);
                }
                else
                {
                    qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime != null);

                    if (qr.Any())
                    {
                        DiemRenLuyen drl1 = qr.SingleOrDefault();

                        drl1.DiemTC1 = drl.DiemTC1;
                        drl1.DiemTC2 = drl.DiemTC2;
                        drl1.DiemTC3 = drl.DiemTC3;
                        drl1.TongDiem = drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3;
                        drl1.DelTime = null;

                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "SinhVien");
                    }
                    else
                    {
                        drl.MaSV = masv;
                        drl.MaHK = mahk;
                        drl.TongDiem = drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3;

                        db.DiemRenLuyens.InsertOnSubmit(drl);
                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "SinhVien");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể gui danh gia, chi tiet loi: " + ex;
                return View(drl);
            }
        }
    }
}