using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class SinhVienController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: SinhVien
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachSinhVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<SinhVien> listsv = db.SinhViens.Where(o => o.DelTime == null).ToList();

            return View(listsv);
        }

        public ActionResult ThemSinhVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new SinhVien());
        }

        [HttpPost]
        public ActionResult ThemSinhVien(SinhVien sv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(sv.MaSV) && !string.IsNullOrEmpty(sv.TenSV))
                {
                    var qr = db.SinhViens.Where(o => o.MaSV == sv.MaSV && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error1"] = "Ma sinh vien da ton tai";
                        return View(sv);
                    }
                    else
                    {
                        qr = db.SinhViens.Where(o => o.MaSV == sv.MaSV && o.DelTime != null);

                        if (qr.Any())
                        {
                            SinhVien sv1 = qr.SingleOrDefault();

                            sv1.MaSV = sv.MaSV;
                            sv1.TenSV = sv.TenSV;
                            sv1.GioiTinh = sv.GioiTinh;
                            sv1.NgaySinh = sv.NgaySinh;
                            sv1.QueQuan = sv.QueQuan;
                            sv1.SoDienThoai = sv.SoDienThoai;
                            sv1.Email = sv.Email;
                            sv1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("Dashboard", "NhanVien");
                        }
                        else
                        {
                            db.SinhViens.InsertOnSubmit(sv);
                            db.SubmitChanges();

                            return RedirectToAction("Dashboard", "NhanVien");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin sinh vien";
                    return View(sv);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi sinh vien, chi tiet loi: " + ex;
                return View(sv);
            }
        }

        public ActionResult SuaSinhVien(string MaSV)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            SinhVien sv = db.SinhViens.FirstOrDefault(o => o.MaSV == MaSV && o.DelTime == null);
            return View(sv);
        }

        [HttpPost]
        public ActionResult SuaSinhVien(SinhVien sv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(sv.MaSV))
                {
                    var qr = db.SinhViens.Where(o => o.MaSV == sv.MaSV && o.DelTime == null);

                    if (qr.Any())
                    {
                        SinhVien sv1 = qr.SingleOrDefault();
                        sv1.TenSV = sv.TenSV;
                        sv1.GioiTinh = sv.GioiTinh;
                        sv1.NgaySinh = sv.NgaySinh;
                        sv1.QueQuan = sv.QueQuan;
                        sv1.SoDienThoai = sv.SoDienThoai;
                        sv1.Email = sv.Email;

                        db.SubmitChanges();

                        return RedirectToAction("Dashboard", "NhanVien");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay sinh vien";
                        return View(sv);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay sinh vien";
                    return View(sv);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin sinh vien, chi tiet loi: " + ex;
                return View(sv);
            }
        }

        public ActionResult XoaSinhVien(string MaSV)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.SinhViens.Where(o => o.MaSV == MaSV && o.DelTime == null);

                SinhVien sv = qr.SingleOrDefault();

                sv.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("Dashboard", "NhanVien");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "NhanVien");
            }
        }

        [HttpPost]
        public JsonResult TimSinhVien(string MaSV, string TenSV)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var dssv = (from item in db.SinhViens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                TenSV = item.TenSV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();

                if (!string.IsNullOrEmpty(MaSV) && string.IsNullOrEmpty(TenSV))
                {
                    dssv = (from item in db.SinhViens.Where(o => o.MaSV == MaSV && o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                TenSV = item.TenSV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }
                else if (!string.IsNullOrEmpty(TenSV) && string.IsNullOrEmpty(MaSV))
                {
                    dssv = (from item in db.SinhViens.Where(o => o.TenSV.Contains(TenSV) && o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                TenSV = item.TenSV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }
                else if (string.IsNullOrEmpty(TenSV) && string.IsNullOrEmpty(MaSV))
                {
                    dssv = (from item in db.SinhViens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                TenSV = item.TenSV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }
                else
                {
                    dssv = (from item in db.SinhViens.Where(o => o.MaSV == MaSV && o.TenSV.Contains(TenSV) && o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                TenSV = item.TenSV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();
                }

                return Json(new { dssv = dssv }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public SinhVien GetSinhVien(string masv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.SinhViens.Where(o => o.MaSV == masv && o.DelTime == null).SingleOrDefault(); ;
            }

            return null;
        }
    }
}