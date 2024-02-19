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
    public class HocBongController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: HocBong
        public ActionResult Index()
        {
            return View();
        }

        public double? TinhDiemTB(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var MHC = new MonHocController();
                List<KetQua> listkq = db.KetQuas.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();

                double? dtb = 0.0;
                int dem = 0;

                foreach (var kq in listkq)
                {
                    dtb = dtb + kq.DiemTongKet * MHC.GetMonHoc(kq.MaMH).SoTin;
                    dem += MHC.GetMonHoc(kq.MaMH).SoTin;
                }

                return dtb / dem;
            }

            return null;
        }

        public ActionResult DanhSachHocBong()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Khoa = db.Khoas.Where(o => o.DelTime == null).ToList();
            ViewBag.HK = db.HocKies.Where(o => o.DelTime == null).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult LapDanhSach(string makhoa, string mahk, double? drlmin, string diemchumin, double? diemhe10min)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");

            else
            {
                double diemchu = 0.0;

                switch(diemchumin)
                {
                    case "F":
                        diemchu = 0.0;
                        break;

                    case "D":
                        diemchu = 4.0;
                        break;

                    case "D+":
                        diemchu = 5.0;
                        break;

                    case "C":
                        diemchu = 5.5;
                        break;

                    case "C+":
                        diemchu = 6.5;
                        break;

                    case "B":
                        diemchu = 7.0;
                        break;

                    case "B+":
                        diemchu = 8.0;
                        break;

                    case "A":
                        diemchu = 8.5;
                        break;

                    case "A+":
                        diemchu = 9.5;
                        break;
                }

                var HKC = new HocKyController();
                var KC = new KhoaController();

                var dshb1 = (from sv in db.SinhViens
                               join kq in db.KetQuas
                               on sv.MaSV equals kq.MaSV
                               join lop in db.Lops
                               on sv.MaLopQuanLy equals lop.MaLop
                               where lop.MaKhoa == makhoa && kq.MaHK == mahk && sv.DelTime == null && kq.DelTime == null && lop.DelTime == null
                               select new SVHBModel
                               {
                                   MaSV = sv.MaSV,
                                   TenSV = sv.TenSV,
                                   MaHK = kq.MaHK,
                                   TenHK = HKC.GetHocKy(kq.MaHK).TenHK,
                                   MaKhoa = lop.MaKhoa,
                                   TenKhoa = KC.GetKhoa(lop.MaKhoa).TenKhoa,
                                   DiemTB = TinhDiemTB(sv.MaSV, kq.MaHK)
                               }).Distinct().ToList();

                List<SVHBModel> dshb = new List<SVHBModel>();

                if (!string.IsNullOrEmpty(makhoa) && !string.IsNullOrEmpty(mahk) && drlmin != null && !string.IsNullOrEmpty(diemchumin) && diemhe10min != null)
                {
                    var DRLC = new DiemRenLuyenController();
                    var KQC = new KetQuaController();

                    foreach (var hb in dshb1)
                    {
                        var chek = true;
                        List<KetQua> listkq = KQC.GetKetQua(hb.MaSV, hb.MaHK);

                        foreach (var kq in listkq)
                        {
                            if (kq.DiemTongKet < diemchu)
                            {
                                chek = false;
                                break;
                            }
                        }

                        if (!chek)
                            continue;

                        if (DRLC.TinhTongDiem(hb.MaSV, hb.MaHK)[2] >= drlmin && hb.DiemTB >= diemhe10min)
                        {
                            dshb.Add(hb);
                        }
                    }
                }

                return Json(new { dshb = dshb }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public List<SVHBModel> LapDanhSachHB(string makhoa, string mahk, double? drlmin, string diemchumin, double? diemhe10min)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");

            else
            {
                double diemchu = 0.0;

                switch (diemchumin)
                {
                    case "F":
                        diemchu = 0.0;
                        break;

                    case "D":
                        diemchu = 4.0;
                        break;

                    case "D+":
                        diemchu = 5.0;
                        break;

                    case "C":
                        diemchu = 5.5;
                        break;

                    case "C+":
                        diemchu = 6.5;
                        break;

                    case "B":
                        diemchu = 7.0;
                        break;

                    case "B+":
                        diemchu = 8.0;
                        break;

                    case "A":
                        diemchu = 8.5;
                        break;

                    case "A+":
                        diemchu = 9.5;
                        break;
                }

                var HKC = new HocKyController();
                var KC = new KhoaController();

                var dshb1 = (from sv in db.SinhViens
                             join kq in db.KetQuas
                             on sv.MaSV equals kq.MaSV
                             join lop in db.Lops
                             on sv.MaLopQuanLy equals lop.MaLop
                             where lop.MaKhoa == makhoa && kq.MaHK == mahk && sv.DelTime == null && kq.DelTime == null && lop.DelTime == null
                             select new SVHBModel
                             {
                                 MaSV = sv.MaSV,
                                 TenSV = sv.TenSV,
                                 MaHK = kq.MaHK,
                                 TenHK = HKC.GetHocKy(kq.MaHK).TenHK,
                                 MaKhoa = lop.MaKhoa,
                                 TenKhoa = KC.GetKhoa(lop.MaKhoa).TenKhoa,
                                 DiemTB = TinhDiemTB(sv.MaSV, kq.MaHK)
                             }).Distinct().ToList();

                List<SVHBModel> dshb = new List<SVHBModel>();

                if (!string.IsNullOrEmpty(makhoa) && !string.IsNullOrEmpty(mahk) && drlmin != null && !string.IsNullOrEmpty(diemchumin) && diemhe10min != null)
                {
                    var DRLC = new DiemRenLuyenController();
                    var KQC = new KetQuaController();

                    foreach (var hb in dshb1)
                    {
                        var chek = true;
                        List<KetQua> listkq = KQC.GetKetQua(hb.MaSV, hb.MaHK);

                        foreach (var kq in listkq)
                        {
                            if (kq.DiemTongKet < diemchu)
                            {
                                chek = false;
                                break;
                            }
                        }

                        if (!chek)
                            continue;

                        if (DRLC.TinhTongDiem(hb.MaSV, hb.MaHK)[2] >= drlmin && hb.DiemTB >= diemhe10min)
                        {
                            dshb.Add(hb);
                        }
                    }
                }

                return dshb;
            }

            return null;
        }

        [HttpPost]
        public void XuatFileExcel(string makhoa, string mahk, double? drlmin, string diemchumin, double? diemhe10min)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<SVHBModel> dshb = LapDanhSachHB(makhoa, mahk, drlmin, diemchumin, diemhe10min);

                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("HocBong");

                Sheet.Cells["A1"].Value = "Ma Sinh Vien";
                Sheet.Cells["B1"].Value = "Ten Sinh Vien";
                Sheet.Cells["C1"].Value = "Khoa";
                Sheet.Cells["D1"].Value = "Hoc Ky";
                Sheet.Cells["E1"].Value = "Diem Trung Binh";

                int row = 2;

                foreach (var hb in dshb)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = hb.MaSV;
                    Sheet.Cells[string.Format("B{0}", row)].Value = hb.TenSV;
                    Sheet.Cells[string.Format("C{0}", row)].Value = hb.TenKhoa;
                    Sheet.Cells[string.Format("D{0}", row)].Value = hb.TenHK;
                    Sheet.Cells[string.Format("E{0}", row)].Value = hb.DiemTB;

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "HocBong.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            }
        }
    }
}