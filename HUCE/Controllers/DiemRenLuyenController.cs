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

        public List<double> TinhTongDiem(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

                List<double> listtongdiem = new List<double>
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
                              MaHK = drl.MaHK,
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

            List<TieuChiDiemRenLuyen> listtc = db.TieuChiDiemRenLuyens.Where(o => o.DelTime == null).ToList();

            List<DiemRenLuyen> listdrl = new List<DiemRenLuyen>();

            foreach (var tc in listtc)
            {
                DiemRenLuyen drl = new DiemRenLuyen();
                drl.MaSV = masv;
                drl.MaHK = mahk;
                drl.MaTC = tc.MaTC;

                listdrl.Add(drl);
            }

            return View(listdrl);
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
                    return View(listdrl);
                }
                else
                {
                    foreach (DiemRenLuyen drl in listdrl)
                    {
                        DiemRenLuyen drl1 = new DiemRenLuyen();
                        drl1.MaSV = masv;
                        drl1.MaTC = drl.MaTC;
                        drl1.MaHK = mahk;
                        drl1.DiemSV = drl.DiemSV;
                        //drl1.DiemCB = drl.DiemSV;
                        //drl1.DiemGVCN = drl.DiemSV;

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

                TempData["Error"] = "Không thể gui danh gia, chi tiet loi: " + ex;
                return View(listdrl);
            }
        }

        public ActionResult SuaDiemRenLuyen(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
            ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

            List<TieuChiDiemRenLuyen> listtc = db.TieuChiDiemRenLuyens.Where(o => o.DelTime == null).ToList();

            List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

            return View(listdrl);
        }

        [HttpPost]
        public ActionResult SuaDiemRenLuyen(List<DiemRenLuyen> listdrl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var masv = listdrl[0].MaSV;
                var mahk = listdrl[0].MaHK;

                var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                if (qr.Any())
                {
                    List<DiemRenLuyen> listdrl1 = qr.ToList();

                    foreach (DiemRenLuyen drl in listdrl)
                    {
                        foreach(DiemRenLuyen drl1 in listdrl1)
                        {
                            if(drl1.MaTC == drl.MaTC)
                            {
                                drl1.DiemSV = drl.DiemSV;
                                drl1.DiemCB = drl.DiemCB;
                                drl1.DiemGVCN = drl.DiemGVCN;

                                db.SubmitChanges();
                            }
                        }
                    }

                    return RedirectToAction("DanhSachDiemRenLuyen", "DiemRenLuyen");
                }
                else
                {
                    ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                    ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                    TempData["Error"] = "Khong tim thay danh gia";
                    return View(listdrl);
                }
            }
            catch (Exception ex)
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                TempData["Error"] = "Không thể sua danh gia, chi tiet loi: " + ex;
                return View(listdrl);
            }
        }

        public ActionResult XoaDiemRenLuyen(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

                foreach(DiemRenLuyen drl in listdrl)
                {
                    drl.DelTime = DateTime.Now;

                    db.SubmitChanges();
                }

                return RedirectToAction("DanhSachDiemRenLuyen", "DiemRenLuyen");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachDiemRenLuyen", "DiemRenLuyen");
            }
        }

        [HttpPost]
        public JsonResult TimDiemRenLuyen(string ttdg)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var HKC = new HocKyController();

                var dsdg = (from sv in db.SinhViens
                               join drl in db.DiemRenLuyens
                               on sv.MaSV equals drl.MaSV
                               where sv.DelTime == null && drl.DelTime == null
                               select new SVDRLModel
                               {
                                   MaSV = sv.MaSV,
                                   TenSV = sv.TenSV,
                                   MaHK = drl.MaHK,
                                   TenHK = HKC.GetHocKy(drl.MaHK).TenHK,
                                   TongDiem = TinhTongDiem(sv.MaSV, drl.MaHK)[2]
                               }).Distinct().ToList();

                if (!string.IsNullOrEmpty(ttdg))
                {
                    if (ttdg.All(char.IsDigit))
                    {
                        dsdg = (from sv in db.SinhViens
                                join drl in db.DiemRenLuyens
                                on sv.MaSV equals drl.MaSV
                                where sv.DelTime == null && drl.DelTime == null && drl.MaSV == ttdg
                                select new SVDRLModel
                                {
                                    MaSV = sv.MaSV,
                                    TenSV = sv.TenSV,
                                    MaHK = drl.MaHK,
                                    TenHK = HKC.GetHocKy(drl.MaHK).TenHK,
                                    TongDiem = TinhTongDiem(sv.MaSV, drl.MaHK)[2]
                                }).Distinct().ToList();
                    }
                    else
                    {
                        dsdg = (from sv in db.SinhViens
                                join drl in db.DiemRenLuyens
                                on sv.MaSV equals drl.MaSV
                                where sv.DelTime == null && drl.DelTime == null && sv.TenSV.Contains(ttdg)
                                select new SVDRLModel
                                {
                                    MaSV = sv.MaSV,
                                    TenSV = sv.TenSV,
                                    MaHK = drl.MaHK,
                                    TenHK = HKC.GetHocKy(drl.MaHK).TenHK,
                                    TongDiem = TinhTongDiem(sv.MaSV, drl.MaHK)[2]
                                }).Distinct().ToList();
                    }
                }

                return Json(new { dsdg = dsdg }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult DanhSachDiemRenLuyenChoCB()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var masvcb = SessionConfig.GetSession();

            var qr = db.SinhViens.SingleOrDefault(o => o.MaSV == masvcb && o.DelTime == null);

            if(qr.CanBoLop == null)
            {
                TempData["Error"] = "Ban khong phai can bo lop";
                return RedirectToAction("Login", "Login");
            }
            else
            {
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

                var HKC = new HocKyController();

                var listdrl = (from sv in db.SinhViens
                               join drl in db.DiemRenLuyens
                               on sv.MaSV equals drl.MaSV
                               where sv.DelTime == null && drl.DelTime == null && sv.MaLopQuanLy == qr.MaLopQuanLy && drl.MaHK == mahk
                               select new SVDRLModel
                               {
                                   MaSV = sv.MaSV,
                                   TenSV = sv.TenSV,
                                   MaHK = drl.MaHK,
                                   TenHK = HKC.GetHocKy(drl.MaHK).TenHK,
                                   TongDiem = TinhTongDiem(sv.MaSV, drl.MaHK)[2]
                               }).Distinct().ToList();

                return View(listdrl);
            }
        }

        public ActionResult DanhSachDiemRenLuyenChoGVCN()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var magvcn = SessionConfig.GetSession();

            var qr = db.GiangViens.SingleOrDefault(o => o.MaGV == magvcn && o.DelTime == null);

            if (qr.GVCN == null)
            {
                TempData["Error"] = "Ban khong phai giao vien chu nhiem";
                return RedirectToAction("Login", "Login");
            }
            else
            {
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

                var HKC = new HocKyController();

                var listdrl = (from sv in db.SinhViens
                               join drl in db.DiemRenLuyens
                               on sv.MaSV equals drl.MaSV
                               where sv.DelTime == null && drl.DelTime == null && sv.MaLopQuanLy == qr.MaLopCN && drl.MaHK == mahk
                               select new SVDRLModel
                               {
                                   MaSV = sv.MaSV,
                                   TenSV = sv.TenSV,
                                   MaHK = drl.MaHK,
                                   TenHK = HKC.GetHocKy(drl.MaHK).TenHK,
                                   TongDiem = TinhTongDiem(sv.MaSV, drl.MaHK)[2]
                               }).Distinct().ToList();

                return View(listdrl);
            }
        }

        public ActionResult ChekDiemRenLuyenChoCB(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var masvcb = SessionConfig.GetSession();

            var qr = db.SinhViens.SingleOrDefault(o => o.MaSV == masvcb && o.DelTime == null);

            if (qr.CanBoLop == null)
            {
                TempData["Error"] = "Ban khong phai can bo lop";
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                List<TieuChiDiemRenLuyen> listtc = db.TieuChiDiemRenLuyens.Where(o => o.DelTime == null).ToList();

                List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

                return View(listdrl);
            }
        }

        [HttpPost]
        public ActionResult ChekDiemRenLuyenChoCB(List<DiemRenLuyen> listdrl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var masvcb = SessionConfig.GetSession();

                var qr1 = db.SinhViens.SingleOrDefault(o => o.MaSV == masvcb && o.DelTime == null);

                if (qr1.CanBoLop == null)
                {
                    TempData["Error"] = "Ban khong phai can bo lop";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    var masv = listdrl[0].MaSV;
                    var mahk = listdrl[0].MaHK;

                    var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                    if (qr.Any())
                    {
                        List<DiemRenLuyen> listdrl1 = qr.ToList();

                        foreach (DiemRenLuyen drl in listdrl)
                        {
                            foreach (DiemRenLuyen drl1 in listdrl1)
                            {
                                if (drl1.MaTC == drl.MaTC)
                                {
                                    drl1.DiemCB = drl.DiemCB;

                                    db.SubmitChanges();
                                }
                            }
                        }

                        return RedirectToAction("DanhSachDiemRenLuyenChoCB", "DiemRenLuyen");
                    }
                    else
                    {
                        ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                        ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                        TempData["Error"] = "Khong tim thay danh gia";
                        return View(listdrl);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                TempData["Error"] = "Không thể gui danh gia, chi tiet loi: " + ex;
                return View(listdrl);
            }
        }

        public ActionResult ChekDiemRenLuyenChoGVCN(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var magvcn = SessionConfig.GetSession();

            var qr = db.GiangViens.SingleOrDefault(o => o.MaGV == magvcn && o.DelTime == null);

            if (qr.GVCN == null)
            {
                TempData["Error"] = "Ban khong phai giao vien chu nhiem";
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                List<TieuChiDiemRenLuyen> listtc = db.TieuChiDiemRenLuyens.Where(o => o.DelTime == null).ToList();

                List<DiemRenLuyen> listdrl = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

                return View(listdrl);
            }
        }

        [HttpPost]
        public ActionResult ChekDiemRenLuyenChoGVCN(List<DiemRenLuyen> listdrl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var magvcn = SessionConfig.GetSession();

                var qr1 = db.GiangViens.SingleOrDefault(o => o.MaGV == magvcn && o.DelTime == null);

                if (qr1.GVCN == null)
                {
                    TempData["Error"] = "Ban khong phai giao vien chu nhiem";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    var masv = listdrl[0].MaSV;
                    var mahk = listdrl[0].MaHK;

                    var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                    if (qr.Any())
                    {
                        List<DiemRenLuyen> listdrl1 = qr.ToList();

                        foreach (DiemRenLuyen drl in listdrl)
                        {
                            foreach (DiemRenLuyen drl1 in listdrl1)
                            {
                                if (drl1.MaTC == drl.MaTC)
                                {
                                    drl1.DiemGVCN = drl.DiemGVCN;

                                    db.SubmitChanges();
                                }
                            }
                        }

                        return RedirectToAction("DanhSachDiemRenLuyenChoGVCN", "DiemRenLuyen");
                    }
                    else
                    {
                        ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                        ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                        TempData["Error"] = "Khong tim thay danh gia";
                        return View(listdrl);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                TempData["Error"] = "Không thể gui danh gia, chi tiet loi: " + ex;
                return View(listdrl);
            }
        }

        public ActionResult XacNhanDRLChoCB(List<DiemRenLuyen> listdrl)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var masv = listdrl[0].MaSV;
                var mahk = listdrl[0].MaHK;

                var qr = db.DiemRenLuyens.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null);

                if (qr.Any())
                {
                    List<DiemRenLuyen> listdrl1 = qr.ToList();

                    foreach (DiemRenLuyen drl in listdrl)
                    {
                        foreach (DiemRenLuyen drl1 in listdrl1)
                        {
                            if (drl1.MaTC == drl.MaTC)
                            {
                                drl1.DiemCB = drl.DiemSV;

                                db.SubmitChanges();
                            }
                        }
                    }

                    return RedirectToAction("ChekDiemRenLuyenChoCB", "DiemRenLuyen");
                }
                else
                {
                    ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                    ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                    TempData["Error"] = "Khong tim thay danh gia";
                    return View(listdrl);
                }
            }
            catch (Exception ex)
            {
                ViewBag.TCCha = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha == null && o.DelTime == null).ToList();
                ViewBag.TC = db.TieuChiDiemRenLuyens.Where(o => o.MaTCCha != null && o.DelTime == null).ToList();

                TempData["Error"] = "Không thể sua danh gia, chi tiet loi: " + ex;
                return View(listdrl);
            }
        }
    }
}