using HUCE.App_Start;
using HUCE.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }

        public ActionResult DanhSachSinhVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<SinhVien> listsv = db.SinhViens.Where(o => o.DelTime == null).ToList();

            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            return View(listsv);
        }

        public ActionResult ThemSinhVien()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            return View(new SinhVien());
        }

        [HttpPost]
        public ActionResult ThemSinhVien(SinhVien sv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            try
            {
                if (!string.IsNullOrEmpty(sv.MaSV) && !string.IsNullOrEmpty(sv.TenSV))
                {
                    var qr = db.SinhViens.Where(o => o.MaSV == sv.MaSV && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error"] = "Ma sinh vien da ton tai";
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
                            sv1.MaLopQuanLy = sv.MaLopQuanLy;
                            sv1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachSinhVien", "SinhVien");
                        }
                        else
                        {
                            db.SinhViens.InsertOnSubmit(sv);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachSinhVien", "SinhVien");
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

        public ActionResult SuaSinhVien(string masv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            SinhVien sv = db.SinhViens.FirstOrDefault(o => o.MaSV == masv && o.DelTime == null);

            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            return View(sv);
        }

        [HttpPost]
        public ActionResult SuaSinhVien(SinhVien sv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            try
            {
                if (!string.IsNullOrEmpty(sv.MaSV) && !string.IsNullOrEmpty(sv.TenSV))
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
                        sv1.MaLopQuanLy = sv.MaLopQuanLy;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachSinhVien", "SinhVien");
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

        public ActionResult XoaSinhVien(string masv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.SinhViens.Where(o => o.MaSV == masv && o.DelTime == null);

                SinhVien sv = qr.SingleOrDefault();

                sv.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachSinhVien", "SinhVien");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachSinhVien", "SinhVien");
            }
        }

        [HttpPost]
        public JsonResult TimSinhVien(string ttsv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var LC = new LopController();

                var dssv = (from item in db.SinhViens.Where(o => o.DelTime == null)
                            select new
                            {
                                MaSV = item.MaSV,
                                TenSV = item.TenSV,
                                GioiTinh = item.GioiTinh,
                                NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                QueQuan = item.QueQuan,
                                SoDienThoai = item.SoDienThoai,
                                Email = item.Email,
                                TenLop = LC.GetLop(item.MaLopQuanLy).TenLop
                            }).ToList();

                if (!string.IsNullOrEmpty(ttsv))
                {
                    if (ttsv.All(char.IsDigit))
                    {
                        dssv = (from item in db.SinhViens.Where(o => o.MaSV == ttsv && o.DelTime == null)
                                select new
                                {
                                    MaSV = item.MaSV,
                                    TenSV = item.TenSV,
                                    GioiTinh = item.GioiTinh,
                                    NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                    QueQuan = item.QueQuan,
                                    SoDienThoai = item.SoDienThoai,
                                    Email = item.Email,
                                    TenLop = LC.GetLop(item.MaLopQuanLy).TenLop
                                }).ToList();
                    }
                    else
                    {
                        dssv = (from item in db.SinhViens.Where(o => o.TenSV.Contains(ttsv) && o.DelTime == null)
                                select new
                                {
                                    MaSV = item.MaSV,
                                    TenSV = item.TenSV,
                                    GioiTinh = item.GioiTinh,
                                    NgaySinh = String.Format("{0: dd/MM/yyyy}", item.NgaySinh),
                                    QueQuan = item.QueQuan,
                                    SoDienThoai = item.SoDienThoai,
                                    Email = item.Email,
                                    TenLop = LC.GetLop(item.MaLopQuanLy).TenLop
                                }).ToList();
                    }
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
                return db.SinhViens.SingleOrDefault(o => o.MaSV == masv && o.DelTime == null);
            }

            return null;
        }

        public ActionResult ChiTietSinhVien(string masv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var sv = db.SinhViens.SingleOrDefault(o => o.MaSV == masv && o.DelTime == null);

            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            return View(sv);
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

                                    var tenlop = worksheet.Cells[row, 8].Value.ToString().Trim();
                                    var qr = db.Lops.Where(o => o.TenLop.Contains(tenlop) && o.DelTime == null);

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
                                            return View("ThemSinhVien", new SinhVien());
                                    }

                                    SinhVien sv = new SinhVien
                                    {
                                        MaSV = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        TenSV = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        GioiTinh = gt,
                                        NgaySinh = DateTime.ParseExact(worksheet.Cells[row, 4].Value.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                        QueQuan = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                        SoDienThoai = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                        Email = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                        MaLopQuanLy = qr.FirstOrDefault().MaLop
                                    };

                                    ThemSinhVien(sv);
                                }
                            }
                        }
                    }

                    if (TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachSinhVien", "SinhVien");
                    }
                    else
                    {
                        return View("ThemSinhVien", new SinhVien());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemSinhVien", new SinhVien());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<SinhVien> listsv = db.SinhViens.Where(o => o.DelTime == null).ToList();

                List<Lop> listlop = db.Lops.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("SinhVien");

                Sheet.Cells["A1"].Value = "Ma Sinh Vien";
                Sheet.Cells["B1"].Value = "Ten Sinh Vien";
                Sheet.Cells["C1"].Value = "Gioi Tinh";
                Sheet.Cells["D1"].Value = "Ngay Sinh";
                Sheet.Cells["E1"].Value = "Que Quan";
                Sheet.Cells["F1"].Value = "So Dien Thoai";
                Sheet.Cells["G1"].Value = "Email";
                Sheet.Cells["H1"].Value = "Lop";

                int row = 2;

                foreach (var sv in listsv)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = sv.MaSV;
                    Sheet.Cells[string.Format("B{0}", row)].Value = sv.TenSV;

                    switch (sv.GioiTinh)
                    {
                        case true:
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Nam";
                            break;

                        case false:
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Nu";
                            break;
                    }

                    Sheet.Cells[string.Format("D{0}", row)].Value = sv.NgaySinh.ToString("dd/MM/yyyy");
                    Sheet.Cells[string.Format("E{0}", row)].Value = sv.QueQuan;
                    Sheet.Cells[string.Format("F{0}", row)].Value = sv.SoDienThoai;
                    Sheet.Cells[string.Format("G{0}", row)].Value = sv.Email;

                    foreach (Lop lop in listlop)
                    {
                        if (sv.MaLopQuanLy == lop.MaLop)
                        {
                            Sheet.Cells[string.Format("H{0}", row)].Value = lop.TenLop;
                        }
                    }

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "SinhVien.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }
    }
}