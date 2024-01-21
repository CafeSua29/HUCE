using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class GiangVienController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: GiangVien
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachGiangVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<GiangVien> listGV = db.GiangViens.Where(o => o.DelTime == null).ToList();

            return View(listGV);
        }

        public ActionResult ThemGiangVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new GiangVien());
        }

        [HttpPost]
        public ActionResult ThemGiangVien(GiangVien gv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(gv.MaGV) && !string.IsNullOrEmpty(gv.TenGV))
                {
                    var qr = db.GiangViens.Where(o => o.MaGV == gv.MaGV && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error1"] = "Ma giang vien da ton tai";
                        return View(gv);
                    }
                    else
                    {
                        qr = db.GiangViens.Where(o => o.MaGV == gv.MaGV && o.DelTime != null);

                        if (qr.Any())
                        {
                            GiangVien gv1 = qr.SingleOrDefault();

                            gv1.MaGV = gv.MaGV;
                            gv1.TenGV = gv.TenGV;
                            gv1.GioiTinh = gv.GioiTinh;
                            gv1.NgaySinh = gv.NgaySinh;
                            gv1.QueQuan = gv.QueQuan;
                            gv1.SoDienThoai = gv.SoDienThoai;
                            gv1.Email = gv.Email;
                            gv1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("Dashboard", "NhanVien");
                        }
                        else
                        {
                            db.GiangViens.InsertOnSubmit(gv);
                            db.SubmitChanges();

                            return RedirectToAction("Dashboard", "NhanVien");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin giang vien";
                    return View(gv);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi giang vien, chi tiet loi: " + ex;
                return View(gv);
            }
        }

        public ActionResult SuaGiangVien(string magv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            GiangVien gv = db.GiangViens.FirstOrDefault(o => o.MaGV == magv && o.DelTime == null);
            return View(gv);
        }

        [HttpPost]
        public ActionResult SuaGiangVien(GiangVien gv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(gv.MaGV))
                {
                    var qr = db.GiangViens.Where(o => o.MaGV == gv.MaGV && o.DelTime == null);

                    if (qr.Any())
                    {
                        GiangVien gv1 = qr.SingleOrDefault();
                        gv1.TenGV = gv.TenGV;
                        gv1.GioiTinh = gv.GioiTinh;
                        gv1.NgaySinh = gv.NgaySinh;
                        gv1.QueQuan = gv.QueQuan;
                        gv1.SoDienThoai = gv.SoDienThoai;
                        gv1.Email = gv.Email;

                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "NhanVien");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay giang vien";
                        return View(gv);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay giang vien";
                    return View(gv);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin giang vien, chi tiet loi: " + ex;
                return View(gv);
            }
        }

        public ActionResult XoaGiangVien(string magv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.GiangViens.Where(o => o.MaGV == magv && o.DelTime == null);

                GiangVien gv = qr.SingleOrDefault();

                gv.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("Dashboard", "NhanVien");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "NhanVien");
            }
        }

        [HttpPost]
        public JsonResult TimGiangVien(string magv, string tengv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var dsgv = (from item in db.GiangViens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaGV = item.MaGV,
                                TenGV = item.TenGV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();

                if (!string.IsNullOrEmpty(magv) && string.IsNullOrEmpty(tengv))
                {
                    dsgv = (from item in db.GiangViens.Where(o => o.MaGV == magv && o.DelTime == null)
                            select new
                            {
                                MaGV = item.MaGV,
                                TenGV = item.TenGV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }
                else if (!string.IsNullOrEmpty(tengv) && string.IsNullOrEmpty(magv))
                {
                    dsgv = (from item in db.GiangViens.Where(o => o.TenGV.Contains(tengv) && o.DelTime == null)
                            select new
                            {
                                MaGV = item.MaGV,
                                TenGV = item.TenGV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }
                else if (string.IsNullOrEmpty(tengv) && string.IsNullOrEmpty(magv))
                {
                    dsgv = (from item in db.GiangViens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaGV = item.MaGV,
                                TenGV = item.TenGV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }
                else
                {
                    dsgv = (from item in db.GiangViens.Where(o => o.MaGV == magv && o.TenGV.Contains(tengv) && o.DelTime == null)
                            select new
                            {
                                MaGV = item.MaGV,
                                TenGV = item.TenGV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }

                return Json(new { dsgv = dsgv }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}