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
    public class GiangVienController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: GiangVien
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
                        TempData["Error"] = "Ma giang vien da ton tai";
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

                            return RedirectToAction("DanhSachGiangVien", "GiangVien");
                        }
                        else
                        {
                            db.GiangViens.InsertOnSubmit(gv);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachGiangVien", "GiangVien");
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

                        return RedirectToAction("DanhSachGiangVien", "GiangVien");
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

                return RedirectToAction("DanhSachGiangVien", "GiangVien");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachGiangVien", "GiangVien");
            }
        }

        [HttpPost]
        public JsonResult TimGiangVien(string ttgv)
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

                if (!string.IsNullOrEmpty(ttgv))
                {
                    if (ttgv.All(char.IsDigit))
                    {
                        dsgv = (from item in db.GiangViens.Where(o => o.MaGV == ttgv && o.DelTime == null)
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
                        dsgv = (from item in db.GiangViens.Where(o => o.TenGV.Contains(ttgv) && o.DelTime == null)
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
                }

                return Json(new { dsgv = dsgv }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public GiangVien GetGiangVien(string magv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.GiangViens.Where(o => o.MaGV == magv && o.DelTime == null).SingleOrDefault(); ;
            }

            return null;
        }

        public ActionResult ChiTietGiangVien(string magv)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var gv = db.GiangViens.Where(o => o.MaGV == magv && o.DelTime == null).SingleOrDefault();

            return View(gv);
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
                                            return View("ThemGiangVien", new GiangVien());
                                    }

                                    GiangVien sv = new GiangVien
                                    {
                                        MaGV = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        TenGV = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        GioiTinh = gt,
                                        NgaySinh = DateTime.ParseExact(worksheet.Cells[row, 4].Value.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                        QueQuan = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                        SoDienThoai = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                        Email = worksheet.Cells[row, 7].Value.ToString().Trim()

                                    };

                                    ThemGiangVien(sv);
                                }
                            }
                        }
                    }

                    if (TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachGiangVien", "GiangVien");
                    }
                    else
                    {
                        return View("ThemGiangVien", new GiangVien());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemGiangVien", new GiangVien());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<GiangVien> listsv = db.GiangViens.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("GiangVien");

                Sheet.Cells["A1"].Value = "Ma Giang Vien";
                Sheet.Cells["B1"].Value = "Ten Giang Vien";
                Sheet.Cells["C1"].Value = "Gioi Tinh";
                Sheet.Cells["D1"].Value = "Ngay Sinh";
                Sheet.Cells["E1"].Value = "Que Quan";
                Sheet.Cells["F1"].Value = "So Dien Thoai";
                Sheet.Cells["G1"].Value = "Email";

                int row = 2;

                foreach (var sv in listsv)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = sv.MaGV;
                    Sheet.Cells[string.Format("B{0}", row)].Value = sv.TenGV;

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

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "GiangVien.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }
    }
}