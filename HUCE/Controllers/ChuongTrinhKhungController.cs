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

            ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();

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
                            var mamh = Request[ctk.MaMH];

                            if (!string.IsNullOrEmpty(mamh))
                            {
                                ctk.DelTime = null;

                                db.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        var listmh = db.MonHocs.Where(o => o.DelTime == null).ToList();

                        foreach (var mh in listmh)
                        {
                            var mamh = Request[mh.MaMH];

                            if (!string.IsNullOrEmpty(mamh))
                            {
                                ChuongTrinhKhung ctk1 = new ChuongTrinhKhung();
                                ctk1.MaKhoa = ctkmodel.MaKhoa;
                                ctk1.MaHK = ctkmodel.MaHK;
                                ctk1.MaMH = mh.MaMH;

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

        public ActionResult SuaChuongTrinhKhung(string makhoa, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
            ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();
            ViewBag.MH = db.MonHocs.Where(o => o.DelTime == null).ToList();

            CTKModel ctkm = new CTKModel();
            ctkm.MaKhoa = makhoa;
            ctkm.MaHK = mahk;

            var listctk = db.ChuongTrinhKhungs.Where(o => o.MaKhoa == makhoa && o.MaHK == mahk && o.DelTime == null).ToList();
            
            foreach(var ctk in listctk)
            {
                ctkm.ListMaMH.Add(ctk.MaMH);
            }

            return View(ctkm);
        }

        [HttpPost]
        public ActionResult SuaChuongTrinhKhung(CTKModel ctkmodel)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if(!string.IsNullOrEmpty(ctkmodel.MaKhoa) && !string.IsNullOrEmpty(ctkmodel.MaHK))
                {
                    var qr = db.ChuongTrinhKhungs.Where(o => o.MaKhoa == ctkmodel.MaKhoa && o.MaHK == ctkmodel.MaHK && o.DelTime == null);

                    if (qr.Any())
                    {
                        var listctk = qr.ToList();

                        foreach(var ctk in listctk)
                        {
                            ctk.DelTime = DateTime.Now;

                            db.SubmitChanges();
                        }

                        var listmh = db.MonHocs.Where(o => o.DelTime == null).ToList();

                        foreach (var mh in listmh)
                        {
                            var mamh = Request[mh.MaMH];

                            if (!string.IsNullOrEmpty(mamh))
                            {
                                var chuongtrinhkhung = db.ChuongTrinhKhungs.SingleOrDefault(o => o.MaKhoa == ctkmodel.MaKhoa && o.MaHK == ctkmodel.MaHK && o.MaMH == mamh && o.DelTime != null);

                                if(chuongtrinhkhung != null)
                                {
                                    chuongtrinhkhung.DelTime = null;

                                    db.SubmitChanges();
                                }
                                else
                                {
                                    ChuongTrinhKhung ctk1 = new ChuongTrinhKhung();
                                    ctk1.MaKhoa = ctkmodel.MaKhoa;
                                    ctk1.MaHK = ctkmodel.MaHK;
                                    ctk1.MaMH = mh.MaMH;

                                    db.ChuongTrinhKhungs.InsertOnSubmit(ctk1);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        return RedirectToAction("DanhSachChuongTrinhKhung", "ChuongTrinhKhung");
                    }
                    else
                    {
                        ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
                        ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();
                        ViewBag.MH = db.MonHocs.Where(o => o.DelTime == null).ToList();

                        TempData["Error"] = "Khong tim thay chuong trinh khung";
                        return View(ctkmodel);
                    }
                }
                else
                {
                    ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
                    ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();
                    ViewBag.MH = db.MonHocs.Where(o => o.DelTime == null).ToList();

                    TempData["Error"] = "Khong tim thay chuong trinh khung";
                    return View(ctkmodel);
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

        public ActionResult XoaChuongTrinhKhung(string makhoa, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.ChuongTrinhKhungs.Where(o => o.MaKhoa == makhoa && o.MaHK == mahk && o.DelTime == null).ToList();

                foreach(var ctk in qr)
                {
                    ctk.DelTime = DateTime.Now;

                    db.SubmitChanges();
                }

                return RedirectToAction("DanhSachChuongTrinhKhung", "ChuongTrinhKhung");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachChuongTrinhKhung", "ChuongTrinhKhung");
            }
        }

        [HttpPost]
        public JsonResult TimChuongTrinhKhung(string makhoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var HKC = new HocKyController();
                var KC = new KhoaController();

                var dsctk = (from ctk in db.ChuongTrinhKhungs
                               where ctk.DelTime == null
                               select new CTKModel
                               {
                                   MaKhoa = ctk.MaKhoa,
                                   TenKhoa = KC.GetKhoa(ctk.MaKhoa).TenKhoa,
                                   MaHK = ctk.MaHK,
                                   TenHK = HKC.GetHocKy(ctk.MaHK).TenHK
                               }).Distinct().ToList();

                if (!string.IsNullOrEmpty(makhoa))
                {
                    dsctk = (from ctk in db.ChuongTrinhKhungs
                             where ctk.MaKhoa == makhoa && ctk.DelTime == null
                             select new CTKModel
                             {
                                 MaKhoa = ctk.MaKhoa,
                                 TenKhoa = KC.GetKhoa(ctk.MaKhoa).TenKhoa,
                                 MaHK = ctk.MaHK,
                                 TenHK = HKC.GetHocKy(ctk.MaHK).TenHK
                             }).Distinct().ToList();
                }

                return Json(new { dsctk = dsctk }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult XemChuongTrinhKhung()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var masv = SessionConfig.GetSession();

            var sv = db.SinhViens.SingleOrDefault(o => o.MaSV  == masv && o.DelTime == null);
            var lop = db.Lops.SingleOrDefault(o => o.MaLop == sv.MaLopQuanLy && o.DelTime == null);
            var khoa = db.Khoas.SingleOrDefault(o => o.MaKhoa == lop.MaKhoa && o.DelTime == null);

            var HKC = new HocKyController();
            var KC = new KhoaController();

            var dsctk = (from ctk in db.ChuongTrinhKhungs
                        where ctk.MaKhoa == khoa.MaKhoa && ctk.DelTime == null
                        select new CTKModel
                         {
                             MaKhoa = ctk.MaKhoa,
                             TenKhoa = KC.GetKhoa(ctk.MaKhoa).TenKhoa,
                             MaHK = ctk.MaHK,
                             TenHK = HKC.GetHocKy(ctk.MaHK).TenHK
                         }).Distinct().ToList();

            foreach(var ctk in dsctk)
            {
                var listmamh = db.ChuongTrinhKhungs.Where(o => o.MaKhoa == ctk.MaKhoa && o.MaHK == ctk.MaHK && o.DelTime == null).ToList();

                foreach(var mamh in listmamh)
                {
                    ctk.ListMaMH.Add(mamh.MaMH);
                }
            }

            return View(dsctk);
        }
    }
}