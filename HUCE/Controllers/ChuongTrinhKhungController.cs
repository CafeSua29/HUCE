using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class ChuongTrinhKhungController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: ChuongTrinhKhung
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachChuongTrinhKhung()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var HKC = new HocKyController();
            var KC = new KhoaController();

            var listctk = (from ctk in db.ChuongTrinhKhungs
                           where ctk.DelTime == null
                           select new CTKModel
                           {
                               MaKhoa = ctk.MaKhoa,
                               TenKhoa = KC.GetKhoa(ctk.MaKhoa).TenKhoa,
                               MaHK = ctk.MaHK,
                               TenHK = HKC.GetHocKy(ctk.MaHK).TenHK
                           }).Distinct().ToList();

            return View(listctk);
        }

        public ActionResult ThemChuongTrinhKhung()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
            ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();
            ViewBag.MH = db.MonHocs.Where(o => o.DelTime == null).ToList();

            return View(new CTKModel());
        }

        [HttpPost]
        public ActionResult ThemChuongTrinhKhung(CTKModel ctkmodel)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.ChuongTrinhKhungs.Where(o => o.MaKhoa == ctkmodel.MaKhoa && o.MaHK == ctkmodel.MaHK && o.DelTime == null);

                if (qr.Any())
                {
                    ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
                    ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();
                    ViewBag.MH = db.MonHocs.Where(o => o.DelTime == null).ToList();

                    TempData["Error"] = "Khoa da co chuong trinh khung roi";
                    return View(ctkmodel);
                }
                else
                {
                    qr = db.ChuongTrinhKhungs.Where(o => o.MaKhoa == ctkmodel.MaKhoa && o.MaHK == ctkmodel.MaHK && o.DelTime != null);

                    if (qr.Any())
                    {
                        var listctk = qr.ToList();

                        foreach(var ctk in listctk)
                        {
                            foreach(var mamh in ctkmodel.ListMaMH)
                            {
                                if(ctk.MaMH == mamh)
                                {
                                    ctk.DelTime = null;

                                    db.SubmitChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var mamh in ctkmodel.ListMaMH)
                        {
                            if(!string.IsNullOrEmpty(mamh))
                            {
                                ChuongTrinhKhung ctk1 = new ChuongTrinhKhung();
                                ctk1.MaKhoa = ctkmodel.MaKhoa;
                                ctk1.MaHK = ctkmodel.MaHK;
                                ctk1.MaMH = mamh;

                                db.ChuongTrinhKhungs.InsertOnSubmit(ctk1);
                                db.SubmitChanges();
                            }
                        }
                    }

                    return RedirectToAction("DanhSachChuongTrinhKhung", "ChuongTrinhKhung");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
                ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();
                ViewBag.MH = db.MonHocs.Where(o => o.DelTime == null).ToList();

                TempData["Error"] = "Không thể them chuong trinh khung, chi tiet loi: " + ex;
                return View(ctkmodel);
            }
        }
    }
}