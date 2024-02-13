using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class QuyenController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: Quyen
        public ActionResult Index()
        {
            return View();
        }

        public Quyen GetQuyen(string maquyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.Quyens.SingleOrDefault(o => o.MaQuyen == maquyen && o.DelTime == null);
            }

            return null;
        }

        public Quyen GetQuyenbyTen(string tenquyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.Quyens.FirstOrDefault(o => o.TenQuyen.Contains(tenquyen) && o.DelTime == null);
            }

            return null;
        }

        public ActionResult DanhSachQuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Quyen> listquyen = db.Quyens.Where(o => o.DelTime == null).ToList();

            return View(listquyen);
        }

        public ActionResult ThemQuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new Quyen());
        }

        [HttpPost]
        public ActionResult ThemQuyen(Quyen quyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(quyen.MaQuyen) && !string.IsNullOrEmpty(quyen.TenQuyen))
                {
                    var qr = db.Quyens.Where(o => o.MaQuyen == quyen.MaQuyen && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error"] = "Ma quyen da ton tai";
                        return View(quyen);
                    }
                    else
                    {
                        qr = db.Quyens.Where(o => o.MaQuyen == quyen.MaQuyen && o.DelTime != null);

                        if (qr.Any())
                        {
                            Quyen quyen1 = qr.SingleOrDefault();

                            quyen1.MaQuyen = quyen.MaQuyen;
                            quyen1.TenQuyen = quyen.TenQuyen;
                            quyen1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachQuyen", "Quyen");
                        }
                        else
                        {
                            db.Quyens.InsertOnSubmit(quyen);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachQuyen", "Quyen");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin quyen";
                    return View(quyen);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi quyen, chi tiet loi: " + ex;
                return View(quyen);
            }
        }

        public ActionResult SuaQuyen(string maquyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            Quyen quyen = db.Quyens.FirstOrDefault(o => o.MaQuyen == maquyen && o.DelTime == null);

            return View(quyen);
        }

        [HttpPost]
        public ActionResult SuaQuyen(Quyen quyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(quyen.MaQuyen) && !string.IsNullOrEmpty(quyen.TenQuyen))
                {
                    var qr = db.Quyens.Where(o => o.MaQuyen == quyen.MaQuyen && o.DelTime == null);

                    if (qr.Any())
                    {
                        Quyen quyen1 = qr.SingleOrDefault();
                        quyen1.TenQuyen = quyen.TenQuyen;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachQuyen", "Quyen");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay quyen";
                        return View(quyen);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay quyen";
                    return View(quyen);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin quyen, chi tiet loi: " + ex;
                return View(quyen);
            }
        }

        public ActionResult XoaQuyen(string maquyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.Quyens.Where(o => o.MaQuyen == maquyen && o.DelTime == null);

                Quyen quyen = qr.SingleOrDefault();

                quyen.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachQuyen", "Quyen");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachQuyen", "Quyen");
            }
        }

        [HttpPost]
        public JsonResult TimQuyen(string ttq)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var dsq = (from item in db.Quyens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaQuyen = item.MaQuyen,
                                TenQuyen = item.TenQuyen
                            }).ToList();

                if (!string.IsNullOrEmpty(ttq))
                {
                    if (ttq.All(char.IsDigit))
                    {
                        dsq = (from item in db.Quyens.Where(o => o.MaQuyen == ttq && o.DelTime == null)
                                select new
                                {
                                    MaQuyen = item.MaQuyen,
                                    TenQuyen = item.TenQuyen
                                }).ToList();
                    }
                    else
                    {
                        dsq = (from item in db.Quyens.Where(o => o.TenQuyen.Contains(ttq) && o.DelTime == null)
                                select new
                                {
                                    MaQuyen = item.MaQuyen,
                                    TenQuyen = item.TenQuyen
                                }).ToList();
                    }
                }

                return Json(new { dsq = dsq }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}