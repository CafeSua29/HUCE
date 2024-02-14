using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class DiemRenLuyenController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: DiemRenLuyen
        public ActionResult Index()
        {
            return View();
        }

        public List<Double> TinhTongDiem(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

                List<Double> listtongdiem = new List<Double>
                {
                    0,
                    0,
                    0
                };

                foreach (var drl in listdrl)
                {
                    listtongdiem[0] += drl.DiemSV;
                    listtongdiem[1] += drl.DiemCB;
                    listtongdiem[2] += drl.DiemGVCN;
                }

                if (listtongdiem[0] < 0)
                    listtongdiem[0] = 0;

                if (listtongdiem[1] < 0)
                    listtongdiem[1] = 0;

                if (listtongdiem[2] < 0)
                    listtongdiem[2] = 0;

                return listtongdiem;
            }

            return null;
        }

        public ActionResult DanhSachDiemRenLuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var HKC = new HocKyController();

            var listdrl = (from sv in db.SinhViens
                          join drl in db.DiemRenLuyens 
                          on sv.MaSV equals drl.MaSV
                          where sv.DelTime == null && drl.DelTime == null
                          select new SVDRLModel
                          {
                              MaSV = sv.MaSV,
                              TenSV = sv.TenSV,
                              TenHK = HKC.GetHocKy(drl.MaHK).TenHK,
                              TongDiem = TinhTongDiem(sv.MaSV, drl.MaHK)[2]
                          }).Distinct().ToList();

            return View(listdrl);
        }

        public ActionResult DanhGiaDiemRenLuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
            ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult DanhGiaDiemRenLuyen(List<DiemRenLuyen> listdrl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var masv = SessionConfig.GetSession();

                var dtn = DateTime.Now;
                var hocky = dtn.Month;
                var namhoc = dtn.Year;

                if (hocky <= 6)
                {
                    hocky = 1;
                }
                else
                {
                    hocky = 2;
                }

                var mahk = namhoc + "_" + hocky;

                var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                if(qr.Any())
                {
                    ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                    ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                    TempData["Error"] = "Ban da danh gia roi";
                    return View();
                }
                else
                {
                    foreach (var drl in listdrl)
                    {
                        DiemRenLuyen drl1 = new DiemRenLuyen();
                        drl1.MaSV = masv;
                        drl1.MaTC = drl.MaTC;
                        drl1.MaHK = mahk;
                        drl1.DiemSV = drl.DiemSV;

                        db.DiemRenLuyens.InsertOnSubmit(drl1);
                        db.SubmitChanges();
                    }

                    return RedirectToAction("Dashboard", "SinhVien");
                }
            }
            catch (Exception ex)
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                TempData["Error"] = "Không thể them moi sinh vien, chi tiet loi: " + ex;
                return View();
            }
        }

        
    }
}