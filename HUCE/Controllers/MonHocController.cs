using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class MonHocController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: MonHoc
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachMonHoc()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<MonHoc> listmh = db.MonHocs.Where(o => o.DelTime == null).ToList();

            return View(listmh);
        }

        public ActionResult ThemMonHoc()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new MonHoc());
        }

        [HttpPost]
        public ActionResult ThemMonHoc(MonHoc mh)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(mh.MaMH) && !string.IsNullOrEmpty(mh.TenMH))
                {
                    var qr = db.MonHocs.Where(o => o.MaMH == mh.MaMH && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error1"] = "Ma mon hoc da ton tai";
                        return View(mh);
                    }
                    else
                    {
                        qr = db.MonHocs.Where(o => o.MaMH == mh.MaMH && o.DelTime != null);

                        if (qr.Any())
                        {
                            MonHoc mh1 = qr.SingleOrDefault();

                            mh1.MaMH = mh.MaMH;
                            mh1.TenMH = mh.TenMH;
                            mh1.SoTin = mh.SoTin;
                            mh1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("Dashboard", "NhanVien");
                        }
                        else
                        {
                            db.MonHocs.InsertOnSubmit(mh);
                            db.SubmitChanges();

                            return RedirectToAction("Dashboard", "NhanVien");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin mon hoc";
                    return View(mh);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi mon hoc, chi tiet loi: " + ex;
                return View(mh);
            }
        }

        public ActionResult SuaMonHoc(string MaMH)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            MonHoc mh = db.MonHocs.FirstOrDefault(o => o.MaMH == MaMH && o.DelTime == null);
            return View(mh);
        }

        [HttpPost]
        public ActionResult SuaMonHoc(MonHoc mh)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(mh.MaMH))
                {
                    var qr = db.MonHocs.Where(o => o.MaMH == mh.MaMH && o.DelTime == null);

                    if (qr.Any())
                    {
                        MonHoc mh1 = qr.SingleOrDefault();
                        mh1.TenMH = mh.TenMH;
                        mh1.SoTin = mh.SoTin;

                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "NhanVien");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay mon hoc";
                        return View(mh);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay mon hoc";
                    return View(mh);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin mon hoc, chi tiet loi: " + ex;
                return View(mh);
            }
        }

        public ActionResult XoaMonHoc(string MaMH)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.MonHocs.Where(o => o.MaMH == MaMH && o.DelTime == null);

                MonHoc mh = qr.SingleOrDefault();

                mh.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("Dashboard", "NhanVien");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "NhanVien");
            }
        }

        [HttpPost]
        public JsonResult TimMonHoc(string MaMH, string TenMH)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var dsmh = (from item in db.MonHocs.Where(o => o.DelTime == null)
                            select new
                            {
                                MaMH = item.MaMH,
                                TenMH = item.TenMH,
                                SoTin = item.SoTin
                            }).ToList();

                if (!string.IsNullOrEmpty(MaMH) && string.IsNullOrEmpty(TenMH))
                {
                    dsmh = (from item in db.MonHocs.Where(o => o.MaMH == MaMH && o.DelTime == null)
                            select new
                            {
                                MaMH = item.MaMH,
                                TenMH = item.TenMH,
                                SoTin = item.SoTin
                            }).ToList();
                }
                else if (!string.IsNullOrEmpty(TenMH) && string.IsNullOrEmpty(MaMH))
                {
                    dsmh = (from item in db.MonHocs.Where(o => o.TenMH.Contains(TenMH) && o.DelTime == null)
                            select new
                            {
                                MaMH = item.MaMH,
                                TenMH = item.TenMH,
                                SoTin = item.SoTin
                            }).ToList();
                }
                else if (string.IsNullOrEmpty(TenMH) && string.IsNullOrEmpty(MaMH))
                {
                    dsmh = (from item in db.MonHocs.Where(o => o.DelTime == null)
                            select new
                            {
                                MaMH = item.MaMH,
                                TenMH = item.TenMH,
                                SoTin = item.SoTin
                            }).ToList();
                }
                else
                {
                    dsmh = (from item in db.MonHocs.Where(o => o.MaMH == MaMH && o.TenMH.Contains(TenMH) && o.DelTime == null)
                            select new
                            {
                                MaMH = item.MaMH,
                                TenMH = item.TenMH,
                                SoTin = item.SoTin
                            }).ToList();
                }

                return Json(new { dsmh = dsmh }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}