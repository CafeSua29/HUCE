using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class LoginController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View(new TaiKhoan());
        }

        [HttpPost]
        public ActionResult Login(TaiKhoan taikhoan)
        {
            try
            {
                if (!string.IsNullOrEmpty(taikhoan.TenTaiKhoan) && !string.IsNullOrEmpty(taikhoan.MatKhau))
                {
                    var qr = db.TaiKhoans.Where(o => o.TenTaiKhoan == taikhoan.TenTaiKhoan && o.DelTime == null);

                    if (qr.Any())
                    {
                        TaiKhoan taikhoan1 = qr.SingleOrDefault();

                        if (taikhoan.MatKhau != taikhoan1.MatKhau)
                        {
                            TempData["Error"] = "Sai mat khau";
                            return View(taikhoan);
                        }
                        else
                        {
                            SessionConfig.DeSession();
                            SessionConfig.SetSession(taikhoan.TenTaiKhoan, taikhoan1.MaQuyen);

                            var listquyen = db.Quyens.Where(o => o.DelTime == null).ToList();

                            foreach (var q in listquyen)
                            {
                                if(taikhoan1.MaQuyen == q.MaQuyen)
                                {
                                    return RedirectToAction("Dashboard", q.QuyenURL);
                                }
                            }

                            TempData["Error"] = "Tai khoan khong ton tai";
                            return View(taikhoan);
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Tai khoan khong ton tai";
                        return View(taikhoan);
                    }
                }
                else
                {
                    TempData["Error"] = "Vui long nhap day du tai khoan, mat khau";
                    return View(taikhoan);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Co loi xay ra trong qua trinh dang nhap, chi tiet loi: " + ex;
                return View(taikhoan);
            }
        }
    }
}