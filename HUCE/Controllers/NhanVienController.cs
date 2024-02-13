using HUCE.App_Start;
using HUCE.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class NhanVienController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: NhanVien
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }

        public NhanVien GetNhanVien(string manv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.NhanViens.SingleOrDefault(o => o.MaNV == manv && o.DelTime == null);
            }

            return null;
        }

        public ActionResult ChiTietNhanVien(string manv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var nv = db.NhanViens.SingleOrDefault(o => o.MaNV == manv && o.DelTime == null);

            return View(nv);
        }

        public ActionResult DanhSachNhanVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<NhanVien> listnv = db.NhanViens.Where(o => o.DelTime == null).ToList();

            return View(listnv);
        }

        public ActionResult ThemNhanVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new NhanVien());
        }

        [HttpPost]
        public ActionResult ThemNhanVien(NhanVien nv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(nv.MaNV) && !string.IsNullOrEmpty(nv.TenNV))
                {
                    var qr = db.NhanViens.Where(o => o.MaNV == nv.MaNV && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error"] = "Ma nhan vien da ton tai";
                        return View(nv);
                    }
                    else
                    {
                        qr = db.NhanViens.Where(o => o.MaNV == nv.MaNV && o.DelTime != null);

                        if (qr.Any())
                        {
                            NhanVien nv1 = qr.SingleOrDefault();

                            nv1.MaNV = nv.MaNV;
                            nv1.TenNV = nv.TenNV;
                            nv1.GioiTinh = nv.GioiTinh;
                            nv1.NgaySinh = nv.NgaySinh;
                            nv1.QueQuan = nv.QueQuan;
                            nv1.SoDienThoai = nv.SoDienThoai;
                            nv1.Email = nv.Email;
                            nv1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachNhanVien", "NhanVien");
                        }
                        else
                        {
                            db.NhanViens.InsertOnSubmit(nv);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachNhanVien", "NhanVien");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin nhan vien";
                    return View(nv);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi nhan vien, chi tiet loi: " + ex;
                return View(nv);
            }
        }

        public ActionResult SuaNhanVien(string manv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            NhanVien nv = db.NhanViens.FirstOrDefault(o => o.MaNV == manv && o.DelTime == null);

            return View(nv);
        }

        [HttpPost]
        public ActionResult SuaNhanVien(NhanVien nv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(nv.MaNV) && !string.IsNullOrEmpty(nv.TenNV))
                {
                    var qr = db.NhanViens.Where(o => o.MaNV == nv.MaNV && o.DelTime == null);

                    if (qr.Any())
                    {
                        NhanVien nv1 = qr.SingleOrDefault();
                        nv1.TenNV = nv.TenNV;
                        nv1.GioiTinh = nv.GioiTinh;
                        nv1.NgaySinh = nv.NgaySinh;
                        nv1.QueQuan = nv.QueQuan;
                        nv1.SoDienThoai = nv.SoDienThoai;
                        nv1.Email = nv.Email;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachNhanVien", "NhanVien");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay nhan vien";
                        return View(nv);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay nhan vien";
                    return View(nv);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin nhan vien, chi tiet loi: " + ex;
                return View(nv);
            }
        }

        public ActionResult XoaNhanVien(string manv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.NhanViens.Where(o => o.MaNV == manv && o.DelTime == null);

                NhanVien NV = qr.SingleOrDefault();

                NV.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachNhanVien", "NhanVien");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachNhanVien", "NhanVien");
            }
        }

        [HttpPost]
        public JsonResult TimNhanVien(string ttnv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var dsnv = (from item in db.NhanViens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaNV = item.MaNV,
                                TenNV = item.TenNV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email
                            }).ToList();

                if (!string.IsNullOrEmpty(ttnv))
                {
                    if (ttnv.All(char.IsDigit))
                    {
                        dsnv = (from item in db.NhanViens.Where(o => o.MaNV == ttnv && o.DelTime == null)
                                select new
                                {
                                    MaNV = item.MaNV,
                                    TenNV = item.TenNV,
                                    GioiTinh = item.GioiTinh,
                                    NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                    QueQuan = item.QueQuan,
                                    SoDienThoai = item.SoDienThoai,
                                    Email = item.Email
                                }).ToList();
                    }
                    else
                    {
                        dsnv = (from item in db.NhanViens.Where(o => o.TenNV.Contains(ttnv) && o.DelTime == null)
                                select new
                                {
                                    MaNV = item.MaNV,
                                    TenNV = item.TenNV,
                                    GioiTinh = item.GioiTinh,
                                    NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                    QueQuan = item.QueQuan,
                                    SoDienThoai = item.SoDienThoai,
                                    Email = item.Email
                                }).ToList();
                    }
                }

                return Json(new { dsnv = dsnv }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult NhapFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");
            else
            {
                try
                {
                    if (Request != null)
                    {
                        HttpPostedFileBase file = Request.Files["file"];

                        if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                        {
                            using (var package = new ExcelPackage(file.InputStream))
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                                int rowCount = worksheet.Dimension.Rows;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var gioitinh = worksheet.Cells[row, 3].Value.ToString().Trim();
                                    var gt = false;

                                    switch (gioitinh)
                                    {
                                        case "Nam":
                                            gt = true;
                                            break;

                                        case "Nu":
                                            gt = false;
                                            break;

                                        default:
                                            TempData["Error"] = "Gioi tinh khong hop le";
                                            return View("ThemNhanVien", new NhanVien());
                                    }

                                    NhanVien nv = new NhanVien
                                    {
                                        MaNV = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        TenNV = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        GioiTinh = gt,
                                        NgaySinh = DateTime.ParseExact(worksheet.Cells[row, 4].Value.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                        QueQuan = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                        SoDienThoai = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                        Email = worksheet.Cells[row, 7].Value.ToString().Trim()
                                    };

                                    ThemNhanVien(nv);
                                }
                            }
                        }
                    }

                    if (TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachNhanVien", "NhanVien");
                    }
                    else
                    {
                        return View("ThemNhanVien", new NhanVien());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemNhanVien", new NhanVien());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<NhanVien> listnv = db.NhanViens.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("NhanVien");

                Sheet.Cells["A1"].Value = "Ma Nhan Vien";
                Sheet.Cells["B1"].Value = "Ten Nhan Vien";
                Sheet.Cells["C1"].Value = "Gioi Tinh";
                Sheet.Cells["D1"].Value = "Ngay Sinh";
                Sheet.Cells["E1"].Value = "Que Quan";
                Sheet.Cells["F1"].Value = "So Dien Thoai";
                Sheet.Cells["G1"].Value = "Email";

                int row = 2;

                foreach (var nv in listnv)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = nv.MaNV;
                    Sheet.Cells[string.Format("B{0}", row)].Value = nv.TenNV;

                    switch (nv.GioiTinh)
                    {
                        case true:
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Nam";
                            break;

                        case false:
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Nu";
                            break;
                    }

                    Sheet.Cells[string.Format("D{0}", row)].Value = nv.NgaySinh.ToString("dd/MM/yyyy");
                    Sheet.Cells[string.Format("E{0}", row)].Value = nv.QueQuan;
                    Sheet.Cells[string.Format("F{0}", row)].Value = nv.SoDienThoai;
                    Sheet.Cells[string.Format("G{0}", row)].Value = nv.Email;

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "NhanVien.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }
    }
}