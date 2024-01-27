using HUCE.App_Start;
using HUCE.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class KhoaController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: Khoa
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachKhoa()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

            return View(listkhoa);
        }

        public ActionResult ThemKhoa()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View(new Khoa());
        }

        [HttpPost]
        public ActionResult ThemKhoa(Khoa khoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(khoa.MaKhoa) && !string.IsNullOrEmpty(khoa.TenKhoa))
                {
                    var qr = db.Khoas.Where(o => o.MaKhoa == khoa.MaKhoa && o.DelTime == null);

                    if (qr.Any())
                    {
                        TempData["Error"] = "Ma khoa da ton tai";
                        return View(khoa);
                    }
                    else
                    {
                        qr = db.Khoas.Where(o => o.MaKhoa == khoa.MaKhoa && o.DelTime != null);

                        if (qr.Any())
                        {
                            Khoa khoa1 = qr.SingleOrDefault();

                            khoa1.MaKhoa = khoa.MaKhoa;
                            khoa1.TenKhoa = khoa.TenKhoa;
                            khoa1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachKhoa", "Khoa");
                        }
                        else
                        {
                            db.Khoas.InsertOnSubmit(khoa);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachKhoa", "Khoa");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin khoa";
                    return View(khoa);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể them moi khoa, chi tiet loi: " + ex;
                return View(khoa);
            }
        }

        public ActionResult SuaKhoa(string makhoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            Khoa khoa = db.Khoas.FirstOrDefault(o => o.MaKhoa == makhoa && o.DelTime == null);
            return View(khoa);
        }

        [HttpPost]
        public ActionResult SuaKhoa(Khoa khoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(khoa.MaKhoa))
                {
                    var qr = db.Khoas.Where(o => o.MaKhoa == khoa.MaKhoa && o.DelTime == null);

                    if (qr.Any())
                    {
                        Khoa khoa1 = qr.SingleOrDefault();
                        khoa1.TenKhoa = khoa.TenKhoa;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachKhoa", "Khoa");
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay khoa";
                        return View(khoa);
                    }
                }
                else
                {
                    TempData["Error"] = "Không tim thay khoa";
                    return View(khoa);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cap nhat thong tin khoa, chi tiet loi: " + ex;
                return View(khoa);
            }
        }

        public ActionResult XoaKhoa(string makhoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.Khoas.Where(o => o.MaKhoa == makhoa && o.DelTime == null);

                Khoa khoa = qr.SingleOrDefault();

                khoa.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachKhoa", "Khoa");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachKhoa", "Khoa");
            }
        }

        [HttpPost]
        public JsonResult TimKhoa(string ttkhoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var dskhoa = (from item in db.Khoas.Where(o => o.DelTime == null)
                            select new
                            {
                                MaKhoa = item.MaKhoa,
                                TenKhoa = item.TenKhoa
                            }).ToList();

                if (!string.IsNullOrEmpty(ttkhoa))
                {
                    if (ttkhoa.All(char.IsDigit))
                    {
                        dskhoa = (from item in db.Khoas.Where(o => o.MaKhoa == ttkhoa && o.DelTime == null)
                                select new
                                {
                                    MaKhoa = item.MaKhoa,
                                    TenKhoa = item.TenKhoa
                                }).ToList();
                    }
                    else
                    {
                        dskhoa = (from item in db.Khoas.Where(o => o.TenKhoa.Contains(ttkhoa) && o.DelTime == null)
                                select new
                                {
                                    MaKhoa = item.MaKhoa,
                                    TenKhoa = item.TenKhoa
                                }).ToList();
                    }
                }

                return Json(new { dskhoa = dskhoa }, JsonRequestBehavior.AllowGet);
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
                                    Khoa khoa = new Khoa
                                    {
                                        MaKhoa = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        TenKhoa = worksheet.Cells[row, 2].Value.ToString().Trim()
                                    };

                                    ThemKhoa(khoa);
                                }
                            }
                        }
                    }

                    if(TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachKhoa", "Khoa");
                    }
                    else
                    {
                        return View("ThemKhoa", new Khoa());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemKhoa", new Khoa());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Khoa");

                Sheet.Cells["A1"].Value = "Ma khoa";
                Sheet.Cells["B1"].Value = "Ten khoa";

                int row = 2;

                foreach (var Khoa in listkhoa)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = Khoa.MaKhoa;
                    Sheet.Cells[string.Format("B{0}", row)].Value = Khoa.TenKhoa;

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "Khoa.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }

        public Khoa GetKhoa(string makhoa)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.Khoas.Where(o => o.MaKhoa == makhoa && o.DelTime == null).SingleOrDefault();
            }

            return null;
        }
    }
}