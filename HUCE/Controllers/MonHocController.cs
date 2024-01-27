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
                    if(mh.SoTin <= 0)
                    {
                        TempData["Error"] = "So tin khong hop le";
                        return View(mh);
                    }
                    else
                    {
                        var qr = db.MonHocs.Where(o => o.MaMH == mh.MaMH && o.DelTime == null);

                        if (qr.Any())
                        {
                            TempData["Error"] = "Ma mon hoc da ton tai";
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

                                return RedirectToAction("DanhSachMonHoc", "MonHoc");
                            }
                            else
                            {
                                db.MonHocs.InsertOnSubmit(mh);
                                db.SubmitChanges();

                                return RedirectToAction("DanhSachMonHoc", "MonHoc");
                            }
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
                    if (mh.SoTin <= 0)
                    {
                        TempData["Error"] = "So tin khong hop le";
                        return View(mh);
                    }
                    else
                    {
                        var qr = db.MonHocs.Where(o => o.MaMH == mh.MaMH && o.DelTime == null);

                        if (qr.Any())
                        {
                            MonHoc mh1 = qr.SingleOrDefault();
                            mh1.TenMH = mh.TenMH;
                            mh1.SoTin = mh.SoTin;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachMonHoc", "MonHoc");
                        }
                        else
                        {
                            TempData["Error"] = "Không tim thay mon hoc";
                            return View(mh);
                        }
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

                return RedirectToAction("DanhSachMonHoc", "MonHoc");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachMonHoc", "MonHoc");
            }
        }

        [HttpPost]
        public JsonResult TimMonHoc(string ttmh)
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

                if (!string.IsNullOrEmpty(ttmh))
                {
                    if (ttmh.All(char.IsDigit))
                    {
                        dsmh = (from item in db.MonHocs.Where(o => o.MaMH == ttmh && o.DelTime == null)
                                select new
                                {
                                    MaMH = item.MaMH,
                                    TenMH = item.TenMH,
                                    SoTin = item.SoTin
                                }).ToList();
                    }
                    else
                    {
                        dsmh = (from item in db.MonHocs.Where(o => o.TenMH.Contains(ttmh) && o.DelTime == null)
                                select new
                                {
                                    MaMH = item.MaMH,
                                    TenMH = item.TenMH,
                                    SoTin = item.SoTin
                                }).ToList();
                    }
                }

                return Json(new { dsmh = dsmh }, JsonRequestBehavior.AllowGet);
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
                                    MonHoc mh = new MonHoc
                                    {
                                        MaMH = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        TenMH = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        SoTin = int.Parse(worksheet.Cells[row, 3].Value.ToString().Trim())
                                    };

                                    ThemMonHoc(mh);
                                }
                            }
                        }
                    }

                    if (TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachMonHoc", "MonHoc");
                    }
                    else
                    {
                        return View("ThemMonHoc", new MonHoc());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemMonHoc", new MonHoc());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<MonHoc> listmh = db.MonHocs.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("MonHoc");

                Sheet.Cells["A1"].Value = "Ma Mon Hoc";
                Sheet.Cells["B1"].Value = "Ten Mon Hoc";
                Sheet.Cells["C1"].Value = "So Tin";

                int row = 2;

                foreach (var mh in listmh)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = mh.MaMH;
                    Sheet.Cells[string.Format("B{0}", row)].Value = mh.TenMH;
                    Sheet.Cells[string.Format("C{0}", row)].Value = mh.SoTin;

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "MonHoc.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }
    }
}