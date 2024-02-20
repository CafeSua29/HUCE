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
    public class TaiKhoanController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: TaiKhoan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachTaiKhoan()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<TaiKhoan> listTK = db.TaiKhoans.Where(o => o.DelTime == null).ToList();

            ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();

            return View(listTK);
        }

        public ActionResult ThemTaiKhoan()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();

            return View(new TaiKhoan());
        }

        [HttpPost]
        public ActionResult ThemTaiKhoan(TaiKhoan tk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(tk.TenTaiKhoan) && !string.IsNullOrEmpty(tk.MatKhau))
                {
                    var qr = db.TaiKhoans.Where(o => o.TenTaiKhoan == tk.TenTaiKhoan && o.DelTime == null);

                    if (qr.Any())
                    {
                        ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();

                        TempData["Error"] = "Tai khoan da ton tai";
                        return View(tk);
                    }
                    else
                    {
                        qr = db.TaiKhoans.Where(o => o.TenTaiKhoan == tk.TenTaiKhoan && o.DelTime != null);

                        if (qr.Any())
                        {
                            TaiKhoan tk1 = qr.SingleOrDefault();

                            tk1.MatKhau = tk.MatKhau;
                            tk1.MaQuyen = tk.MaQuyen;
                            tk1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
                        }
                        else
                        {
                            db.TaiKhoans.InsertOnSubmit(tk);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
                        }
                    }
                }
                else
                {
                    ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();

                    TempData["Error"] = "Vui long nhap day du thong tin tai khoan";
                    return View(tk);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();

                TempData["Error"] = "Không thể them moi tai khoan, chi tiet loi: " + ex;
                return View(tk);
            }
        }

        public ActionResult SuaTaiKhoan(string tentk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            TaiKhoan tk = db.TaiKhoans.FirstOrDefault(o => o.TenTaiKhoan == tentk && o.DelTime == null);

            ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
            ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

            if(tk.MaQuyen == "3" || tk.MaQuyen == "6")
                ViewBag.LCN = db.GiangViens.SingleOrDefault(o => o.MaGV == tentk && o.DelTime == null).MaLopCN;

            return View(tk);
        }

        [HttpPost]
        public ActionResult SuaTaiKhoan(TaiKhoan tk, string malopcn)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(tk.TenTaiKhoan) && !string.IsNullOrEmpty(tk.MatKhau))
                {
                    var qr = db.TaiKhoans.Where(o => o.TenTaiKhoan == tk.TenTaiKhoan && o.DelTime == null);

                    if (qr.Any())
                    {
                        TaiKhoan tk1 = qr.SingleOrDefault();
                        tk1.MatKhau = tk.MatKhau;
                        tk1.MaQuyen = tk.MaQuyen;

                        db.SubmitChanges();

                        if (!string.IsNullOrEmpty(malopcn))
                        {
                            var qr1 = db.GiangViens.Where(o => o.MaGV == tk.TenTaiKhoan && o.DelTime == null);

                            if (qr1.Any())
                            {
                                GiangVien gv = qr1.SingleOrDefault();

                                if (!string.IsNullOrEmpty(gv.GVCN))
                                {
                                    gv.MaLopCN = malopcn;

                                    db.SubmitChanges();
                                }
                            }
                        }

                        return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
                    }
                    else
                    {
                        ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                        ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

                        if (tk.MaQuyen == "3" || tk.MaQuyen == "6")
                            ViewBag.LCN = db.GiangViens.SingleOrDefault(o => o.MaGV == tk.TenTaiKhoan && o.DelTime == null).MaLopCN;

                        TempData["Error"] = "Không tim thay tai khoan";
                        return View(tk);
                    }
                }
                else
                {
                    ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                    ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

                    if (tk.MaQuyen == "3" || tk.MaQuyen == "6")
                        ViewBag.LCN = db.GiangViens.SingleOrDefault(o => o.MaGV == tk.TenTaiKhoan && o.DelTime == null).MaLopCN;

                    TempData["Error"] = "Không tim thay tai khoan";
                    return View(tk);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                ViewBag.Lop = db.Lops.Where(o => o.DelTime == null).ToList();

                if (tk.MaQuyen == "3" || tk.MaQuyen == "6")
                    ViewBag.LCN = db.GiangViens.SingleOrDefault(o => o.MaGV == tk.TenTaiKhoan && o.DelTime == null).MaLopCN;

                TempData["Error"] = "Không thể cap nhat thong tin tai khoan, chi tiet loi: " + ex;
                return View(tk);
            }
        }

        public ActionResult XoaTaiKhoan(string tentk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.TaiKhoans.Where(o => o.TenTaiKhoan == tentk && o.DelTime == null);

                TaiKhoan tk = qr.SingleOrDefault();

                tk.DelTime = DateTime.Now;

                db.SubmitChanges();

                if(tentk == SessionConfig.GetSession())
                {
                    SessionConfig.DeSession();

                    return RedirectToAction("Login", "Login");
                }
                else
                    return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
            }
        }

        [HttpPost]
        public JsonResult TimTaiKhoan(string tentk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var QC = new QuyenController();

                var dstk = (from item in db.TaiKhoans.Where(o => o.DelTime == null)
                            select new
                            {
                                TenTaiKhoan = item.TenTaiKhoan,
                                MatKhau = item.MatKhau,
                                TenQuyen = QC.GetQuyen(item.MaQuyen).TenQuyen
                            }).ToList();

                if (!string.IsNullOrEmpty(tentk))
                {
                    dstk = (from item in db.TaiKhoans.Where(o => o.TenTaiKhoan.Contains(tentk) && o.DelTime == null)
                            select new
                            {
                                TenTaiKhoan = item.TenTaiKhoan,
                                MatKhau = item.MatKhau,
                                TenQuyen = QC.GetQuyen(item.MaQuyen).TenQuyen
                            }).ToList();
                }

                return Json(new { dstk = dstk }, JsonRequestBehavior.AllowGet);
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

                                var QC = new QuyenController();

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var Quyen = worksheet.Cells[row, 3].Value.ToString().Trim();

                                    switch (Quyen)
                                    {
                                        case "Admin":
                                            Quyen = "1";
                                            break;

                                        case "Nhan Vien":
                                            Quyen = "2";
                                            break;

                                        case "Giang Vien":
                                            Quyen = "3";
                                            break;

                                        case "Sinh Vien":
                                            Quyen = "4";
                                            break;

                                        case "Lop Truong":
                                            Quyen = "5";
                                            break;

                                        case "Chu Nhiem":
                                            Quyen = "6";
                                            break;

                                        default:
                                            TempData["Error"] = "Quyen khong hop le";
                                            return View("ThemTaiKhoan", new TaiKhoan());
                                    }

                                    TaiKhoan tk = new TaiKhoan
                                    {
                                        TenTaiKhoan = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        MatKhau = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        MaQuyen = Quyen
                                    };

                                    ThemTaiKhoan(tk);
                                }
                            }
                        }
                    }

                    if (TempData["Error"] == null)
                    {
                        return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
                    }
                    else
                    {
                        return View("ThemTaiKhoan", new TaiKhoan());
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể them moi danh sach, chi tiet loi: " + ex;
                    return View("ThemTaiKhoan", new TaiKhoan());
                }
            }
        }

        public void XuatFileExcel()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<TaiKhoan> listTK = db.TaiKhoans.Where(o => o.DelTime == null).ToList();

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("TaiKhoan");

                Sheet.Cells["A1"].Value = "Ten Tai Khoan";
                Sheet.Cells["B1"].Value = "Mat Khau";
                Sheet.Cells["C1"].Value = "Quyen";

                int row = 2;

                foreach (var tk in listTK)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = tk.TenTaiKhoan;
                    Sheet.Cells[string.Format("B{0}", row)].Value = tk.MatKhau;

                    switch (tk.MaQuyen)
                    {
                        case "1":
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Admin";
                            break;

                        case "2":
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Nhan Vien";
                            break;

                        case "3":
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Giang Vien";
                            break;

                        case "4":
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Sinh Vien";
                            break;

                        case "5":
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Lop Truong";
                            break;

                        case "6":
                            Sheet.Cells[string.Format("C{0}", row)].Value = "Chu Nhiem";
                            break;
                    }

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "TaiKhoan.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }

        public ActionResult DoiMatKhau()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            return View();
        }

        [HttpPost]
        public ActionResult DoiMatKhau(string currpass, string newpass, string confpass)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(currpass) && !string.IsNullOrEmpty(newpass) && !string.IsNullOrEmpty(confpass))
                {
                    var tentk = SessionConfig.GetSession();
                    var qr = db.TaiKhoans.Where(o => o.TenTaiKhoan == tentk && o.DelTime == null);

                    if (qr.Any())
                    {
                        TaiKhoan tk = qr.SingleOrDefault();
                        
                        if (currpass != tk.MatKhau)
                        {
                            TempData["Error"] = "Mat khau hien tai khong dung";
                            return View();
                        }
                        else
                        {
                            if(newpass != confpass)
                            {
                                TempData["Error"] = "Xac nhan mat khau khong dung";
                                return View();
                            }
                            else
                            {
                                if(tk.MatKhau == newpass)
                                {
                                    TempData["Error"] = "Mat khau moi giong mat khau cu";
                                    return View();
                                }
                                else
                                {
                                    tk.MatKhau = newpass;

                                    db.SubmitChanges();

                                    SessionConfig.DeSession();
                                    return RedirectToAction("Login", "Login");
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Không tim thay tai khoan";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du thong tin ";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể doi mat khau, chi tiet loi: " + ex;
                return View();
            }
        }

        public ActionResult DangXuat()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            SessionConfig.DeSession();

            return RedirectToAction("Login", "Login");
        }
    }
}