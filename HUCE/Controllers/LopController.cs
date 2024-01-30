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
    public class LopController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: Lop
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachLop()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Lop> listlop = db.Lops.Where(o => o.DelTime == null).ToList();
            List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

            ViewBag.Khoa = listkhoa;

            return View(listlop);
        }

        public ActionResult ThemLop()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

            ViewBag.Khoa = listkhoa;

            return View(new Lop());
        }

        [HttpPost]
        public ActionResult ThemLop(Lop lop)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

            try
            {
                if (!string.IsNullOrEmpty(lop.MaLop) && !string.IsNullOrEmpty(lop.TenLop))
                {
                    var qr = db.Lops.Where(o => o.MaLop == lop.MaLop && o.DelTime == null);

                    if (qr.Any())
                    {
                        ViewBag.Khoa = listkhoa;
                        TempData["Error"] = "Ma lop da ton tai";
                        return View(lop);
                    }
                    else
                    {
                        qr = db.Lops.Where(o => o.MaLop == lop.MaLop && o.DelTime != null);

                        if (qr.Any())
                        {
                            Lop lop1 = qr.SingleOrDefault();

                            lop1.MaLop = lop.MaLop;
                            lop1.TenLop = lop.TenLop;
                            lop1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachLop", "Lop");
                        }
                        else
                        {
                            db.Lops.InsertOnSubmit(lop);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachLop", "Lop");
                        }
                    }
                }
                else
                {
                    ViewBag.Khoa = listkhoa;
                    TempData["Error"] = "Vui long nhap day du thong tin lop";
                    return View(lop);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Khoa = listkhoa;
                TempData["Error"] = "Không thể them moi lop, chi tiet loi: " + ex;
                return View(lop);
            }
        }

        public ActionResult SuaLop(string malop)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

            ViewBag.Khoa = listkhoa;

            Lop lop = db.Lops.FirstOrDefault(o => o.MaLop == malop && o.DelTime == null);
            return View(lop);
        }

        [HttpPost]
        public ActionResult SuaLop(Lop lop)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<Khoa> listkhoa = db.Khoas.Where(o => o.DelTime == null).ToList();

            try
            {
                if (!string.IsNullOrEmpty(lop.MaLop))
                {
                    var qr = db.Lops.Where(o => o.MaLop == lop.MaLop && o.DelTime == null);

                    if (qr.Any())
                    {
                        Lop lop1 = qr.SingleOrDefault();
                        lop1.TenLop = lop.TenLop;
                        lop1.MaKhoa = lop.MaKhoa;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachLop", "Lop");
                    }
                    else
                    {
                        ViewBag.Khoa = listkhoa;
                        TempData["Error"] = "Không tim thay lop";
                        return View(lop);
                    }
                }
                else
                {
                    ViewBag.Khoa = listkhoa;
                    TempData["Error"] = "Không tim thay lop";
                    return View(lop);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Khoa = listkhoa;
                TempData["Error"] = "Không thể cap nhat thong tin lop, chi tiet loi: " + ex;
                return View(lop);
            }
        }

        public ActionResult XoaLop(string malop)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.Lops.Where(o => o.MaLop == malop && o.DelTime == null);

                Lop lop = qr.SingleOrDefault();

                lop.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachLop", "Lop");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachLop", "Lop");
            }
        }

        [HttpPost]
        public JsonResult TimLop(string ttlop)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var KC = new KhoaController();

                var dslop = (from item in db.Lops.Where(o => o.DelTime == null)
                              select new
                              {
                                  MaLop = item.MaLop,
                                  TenLop = item.TenLop,
                                  TenKhoa = KC.GetKhoa(item.MaKhoa).TenKhoa
                              }).ToList();

                if (!string.IsNullOrEmpty(ttlop))
                {
                    if (ttlop.All(char.IsDigit))
                    {
                        dslop = (from item in db.Lops.Where(o => o.MaLop == ttlop && o.DelTime == null)
                                  select new
                                  {
                                      MaLop = item.MaLop,
                                      TenLop = item.TenLop,
                                      TenKhoa = KC.GetKhoa(item.MaKhoa).TenKhoa
                                  }).ToList();
                    }
                    else
                    {
                        dslop = (from item in db.Lops.Where(o => o.TenLop.Contains(ttlop) && o.DelTime == null)
                                  select new
                                  {
                                      MaLop = item.MaLop,
                                      TenLop = item.TenLop,
                                      TenKhoa = KC.GetKhoa(item.MaKhoa).TenKhoa
                                  }).ToList();
                    }
                }

                return Json(new { dslop = dslop }, JsonRequestBehavior.AllowGet);
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
                                    var tenkhoa = worksheet.Cells[row, 3].Value.ToString().Trim();
                                    var qr = db.Khoas.Where(o => o.TenKhoa.Contains(tenkhoa) && o.DelTime == null);

                                    if(qr.Any())
                                    {
                                        Lop lop = new Lop
                                        {
                                            MaLop = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                            TenLop = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                            MaKhoa = qr.SingleOrDefault().MaKhoa
                                        };

                                        ThemLop(lop);
                                    }
                                    else
                                    {
                                        TempData["Error"] = "Khoa khong ton tai";
                                        return View("ThemLop", new Lop());
                                    }
                                }
                            }
                        }
                    }

                    if (TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachLop", "Lop");
                    }
                    else
                    {
                        return View("ThemLop", new Lop());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemLop", new Lop());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<Lop> listlop = db.Lops.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Lop");

                Sheet.Cells["A1"].Value = "Ma Lop";
                Sheet.Cells["B1"].Value = "Ten Lop";
                Sheet.Cells["C1"].Value = "Khoa";

                int row = 2;

                foreach (var Lop in listlop)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = Lop.MaLop;
                    Sheet.Cells[string.Format("B{0}", row)].Value = Lop.TenLop;

                    var qr = db.Khoas.Where(o => o.MaKhoa == Lop.MaKhoa && o.DelTime == null).SingleOrDefault();

                    Sheet.Cells[string.Format("C{0}", row)].Value = qr.TenKhoa;

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "Lop.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }

        public Lop GetLop(string malop)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.Lops.Where(o => o.MaLop == malop && o.DelTime == null).SingleOrDefault();
            }

            return null;
        }
    }
}