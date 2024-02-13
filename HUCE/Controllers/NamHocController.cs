using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class NamHocController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: NamHoc
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachNamHoc()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<NamHoc> listnh = db.NamHocs.Where(o => o.DelTime == null).ToList();

            return View(listnh);
        }

        public ActionResult ThemNamHoc()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new NamHoc());
        }

        [HttpPost]
        public ActionResult ThemNamHoc(NamHoc nh)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(nh.MaNH) && !string.IsNullOrEmpty(nh.TenNH))
                {
                    var qr = db.NamHocs.Where(o => o.MaNH == nh.MaNH && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error"] = "Ma nam hoc da ton tai";
                        return View(nh);
                    }
                    else
                    {
                        qr = db.NamHocs.Where(o => o.MaNH == nh.MaNH && o.DelTime != null);

                        if (qr.Any())
                        {
                            NamHoc nh1 = qr.SingleOrDefault();

                            nh1.MaNH = nh.MaNH;
                            nh1.TenNH = nh.TenNH;
                            nh1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachNamHoc", "NamHoc");
                        }
                        else
                        {
                            db.NamHocs.InsertOnSubmit(nh);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachNamHoc", "NamHoc");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin nam hoc";
                    return View(nh);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi nam hoc, chi tiet loi: " + ex;
                return View(nh);
            }
        }

        public ActionResult SuaNamHoc(string manh)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            NamHoc nh = db.NamHocs.FirstOrDefault(o => o.MaNH == manh && o.DelTime == null);

            return View(nh);
        }

        [HttpPost]
        public ActionResult SuaNamHoc(NamHoc nh)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(nh.MaNH) && !string.IsNullOrEmpty(nh.TenNH))
                {
                    var qr = db.NamHocs.Where(o => o.MaNH == nh.MaNH && o.DelTime == null);

                    if (qr.Any())
                    {
                        NamHoc nh1 = qr.SingleOrDefault();
                        nh1.TenNH = nh.TenNH;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachNamHoc", "NamHoc");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay nam hoc";
                        return View(nh);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay nam hoc";
                    return View(nh);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin nam hoc, chi tiet loi: " + ex;
                return View(nh);
            }
        }

        public ActionResult XoaNamHoc(string manh)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.NamHocs.Where(o => o.MaNH == manh && o.DelTime == null);

                NamHoc nh = qr.SingleOrDefault();

                nh.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachNamHoc", "NamHoc");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachNamHoc", "NamHoc");
            }
        }
    }
}