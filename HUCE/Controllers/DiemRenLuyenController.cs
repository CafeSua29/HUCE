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

            List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.DelTime == null).ToList();

            ViewBag.SV = db.SinhViens.Where(o => o.DelTime == null).ToList();

            return View(listdrl);
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

                        if(drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3 < 0)
                        {
                            drl1.TongDiem = 0;
                        }
                        else
                        {
                            drl1.TongDiem = drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3;
                        }

                        drl1.DelTime = null;

                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "DiemRenLuyen");
                    }
                    else
                    {
                        drl.MaSV = masv;
                        drl.MaHK = mahk;

                        if (drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3 < 0)
                        {
                            drl.TongDiem = 0;
                        }
                        else
                        {
                            drl.TongDiem = drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3;
                        }

                        db.DiemRenLuyens.InsertOnSubmit(drl);
                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "DiemRenLuyen");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể gui danh gia, chi tiet loi: " + ex;
                return View(drl);
            }
        }

        public ActionResult SuaDiemRenLuyen(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            DiemRenLuyen drl = db.DiemRenLuyens.FirstOrDefault(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

            return View(drl);
        }

        [HttpPost]
        public ActionResult SuaDiemRenLuyen(DiemRenLuyen drl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(drl.MaSV) && !string.IsNullOrEmpty(drl.MaHK))
                {
                    var qr = db.DiemRenLuyens.Where(o => o.MaSV == drl.MaSV && o.MaHK == drl.MaHK && o.DelTime == null);

                    if (qr.Any())
                    {
                        DiemRenLuyen drl1 = qr.SingleOrDefault();

                        drl1.DiemTC1 = drl.DiemTC1;
                        drl1.DiemTC2 = drl.DiemTC2;
                        drl1.DiemTC3 = drl.DiemTC3;

                        if (drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3 < 0)
                        {
                            drl1.TongDiem = 0;
                        }
                        else
                        {
                            drl1.TongDiem = drl.DiemTC1 + drl.DiemTC2 + drl.DiemTC3;
                        }

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachDiemRenLuyen", "DiemRenLuyen");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay danh gia";
                        return View(drl);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay danh gia";
                    return View(drl);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin danh gia, chi tiet loi: " + ex;
                return View(drl);
            }
        }

        public ActionResult XoaDiemRenLuyen(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                DiemRenLuyen drl = qr.SingleOrDefault();

                drl.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachDiemRenLuyen", "DiemRenLuyen");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachDiemRenLuyen", "DiemRenLuyen");
            }
        }

        [HttpPost]
        public JsonResult TimDiemRenLuyen(string ttdg)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var SVC = new SinhVienController();

                var dsdg = (from item in db.DiemRenLuyens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                MaHK = item.MaHK,
                                TenSV = SVC.GetSinhVien(item.MaSV).TenSV,
                                TongDiem = item.TongDiem
                            }).ToList();

                if (!string.IsNullOrEmpty(ttdg))
                {
                    if (ttdg.All(char.IsDigit))
                    {
                        dsdg = (from item in db.DiemRenLuyens.Where(o => o.MaSV == ttdg && o.DelTime == null)
                                select new
                                {
                                    MaSV = item.MaSV,
                                    MaHK = item.MaHK,
                                    TenSV = SVC.GetSinhVien(item.MaSV).TenSV,
                                    TongDiem = item.TongDiem
                                }).ToList();
                    }
                    else
                    {
                        var qr = db.SinhViens.Where(o => o.TenSV.Contains(ttdg) && o.DelTime == null).ToList();

                        dsdg.Clear();

                        foreach(SinhVien sv in qr)
                        {
                             var sv1 = (from item in db.DiemRenLuyens.Where(o => o.MaSV == sv.MaSV && o.DelTime == null)
                                        select new
                                        {
                                            MaSV = item.MaSV,
                                            MaHK = item.MaHK,
                                            TenSV = SVC.GetSinhVien(item.MaSV).TenSV,
                                            TongDiem = item.TongDiem
                                        });

                            dsdg.Add(sv1.SingleOrDefault());
                        }
                    }
                }

                return Json(new { dsdg = dsdg }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}