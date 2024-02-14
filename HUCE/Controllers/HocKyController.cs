using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class HocKyController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: HocKy
        public ActionResult Index()
        {
            return View();
        }

        public HocKy GetHocKy(string MaHK)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.HocKies.SingleOrDefault(o => o.MaHK == MaHK && o.DelTime == null);
            }

            return null;
        }

        public ActionResult DanhSachHocKy()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<HocKy> listhk = db.HocKies.Where(o => o.DelTime == null).ToList();
            List<NamHoc> listnh = db.NamHocs.Where(o => o.DelTime == null).ToList();

            ViewBag.NH = listnh;

            return View(listhk);
        }

        public ActionResult ThemHocKy()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<NamHoc> listnh = db.NamHocs.Where(o => o.DelTime == null).ToList();

            ViewBag.NH = listnh;

            return View(new HocKy());
        }

        [HttpPost]
        public ActionResult ThemHocKy(HocKy hk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<NamHoc> listnh = db.NamHocs.Where(o => o.DelTime == null).ToList();

            try
            {
                if (!string.IsNullOrEmpty(hk.MaHK) && !string.IsNullOrEmpty(hk.TenHK))
                {
                    var qr = db.HocKies.Where(o => o.MaHK == hk.MaHK && o.DelTime == null);

                    if (qr.Any())
                    {
                        ViewBag.NH = listnh;
                        TempData["Error"] = "Ma hoc ky da ton tai";
                        return View(hk);
                    }
                    else
                    {
                        qr = db.HocKies.Where(o => o.MaHK == hk.MaHK && o.DelTime != null);

                        if (qr.Any())
                        {
                            HocKy hk1 = qr.SingleOrDefault();

                            hk1.MaHK = hk.MaHK;
                            hk1.TenHK = hk.TenHK;
                            hk1.MaNH = hk.MaNH;
                            hk1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachHocKy", "HocKy");
                        }
                        else
                        {
                            db.HocKies.InsertOnSubmit(hk);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachHocKy", "HocKy");
                        }
                    }
                }
                else
                {
                    ViewBag.NH = listnh;
                    TempData["Error"] = "Vui long nhap day du thong tin hoc ky";
                    return View(hk);
                }
            }
            catch (Exception ex)
            {
                ViewBag.NH = listnh;
                TempData["Error"] = "Không thể them moi hoc ky, chi tiet loi: " + ex;
                return View(hk);
            }
        }

        public ActionResult SuaHocKy(string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<NamHoc> listnh = db.NamHocs.Where(o => o.DelTime == null).ToList();

            ViewBag.NH = listnh;

            HocKy HocKy = db.HocKies.FirstOrDefault(o => o.MaHK == mahk && o.DelTime == null);

            return View(HocKy);
        }

        [HttpPost]
        public ActionResult SuaHocKy(HocKy hk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<NamHoc> listnh = db.NamHocs.Where(o => o.DelTime == null).ToList();

            try
            {
                if (!string.IsNullOrEmpty(hk.MaHK) && !string.IsNullOrEmpty(hk.TenHK))
                {
                    var qr = db.HocKies.Where(o => o.MaHK == hk.MaHK && o.DelTime == null);

                    if (qr.Any())
                    {
                        HocKy hk1 = qr.SingleOrDefault();
                        hk1.TenHK = hk.TenHK;
                        hk1.MaNH = hk.MaNH;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachHocKy", "HocKy");
                    }
                    else
                    {
                        ViewBag.NH = listnh;
                        TempData["Error"] = "Không tim thay hoc ky";
                        return View(hk);
                    }
                }
                else
                {
                    ViewBag.NH = listnh;
                    TempData["Error"] = "Không tim thay hoc ky";
                    return View(hk);
                }
            }
            catch (Exception ex)
            {
                ViewBag.NH = listnh;
                TempData["Error"] = "Không thể cap nhat thong tin hoc ky, chi tiet loi: " + ex;
                return View(hk);
            }
        }

        public ActionResult XoaHocKy(string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.HocKies.Where(o => o.MaHK == mahk && o.DelTime == null);

                HocKy hk = qr.SingleOrDefault();

                hk.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachHocKy", "HocKy");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachHocKy", "HocKy");
            }
        }
    }
}