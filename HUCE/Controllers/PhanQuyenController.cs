using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class PhanQuyenController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: PhanQuyen
        public ActionResult Index()
        {
            return View();
        }

        public List<List<DanhMuc>> GetPhanQuyen(string maquyen)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                List<PhanQuyen> listpq = db.PhanQuyens.Where(o => o.DelTime == null && o.MaQuyen == maquyen).ToList();

                List<DanhMuc> listdmcha = new List<DanhMuc>();
                List<DanhMuc> listdmcon = new List<DanhMuc>();

                foreach (PhanQuyen pq in listpq)
                {
                    var qr = db.DanhMucs.SingleOrDefault(o => o.DelTime == null && o.MaDM == pq.MaDM && o.MaDMCha == null);

                    if(qr != null)
                    {
                        if (!listdmcha.Contains(qr))
                        {
                            listdmcha.Add(qr);
                        }
                    }
                }

                foreach (DanhMuc dm in listdmcha)
                {
                    var qr = db.DanhMucs.Where(o => o.DelTime == null && o.MaDMCha == dm.MaDM).ToList();

                    foreach (DanhMuc dmc in qr)
                    {
                        foreach (PhanQuyen pq in listpq)
                        {
                            if(pq.MaDM == dmc.MaDM)
                            {
                                if (!listdmcon.Contains(dmc))
                                {
                                    listdmcon.Add(dmc);
                                }
                            }
                        }
                    }
                }

                List<List<DanhMuc>> listdm = new List<List<DanhMuc>>
                {
                    listdmcha,
                    listdmcon
                };

                return listdm;
            }

            return null;
        }

        public ActionResult DanhSachPhanQuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            List<PhanQuyen> listpq = db.PhanQuyens.Where(o => o.DelTime == null).ToList();

            ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
            ViewBag.DM = db.DanhMucs.Where(o => o.MaDMCha == null && o.DelTime == null).ToList();

            return View(listpq);
        }

        public ActionResult PhanQuyen()
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
            ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

            return View(new PhanQuyen());
        }

        [HttpPost]
        public ActionResult PhanQuyen(PhanQuyen pq)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(pq.MaQuyen) && !string.IsNullOrEmpty(pq.MaDM))
                {
                    var qr = db.PhanQuyens.Where(o => o.MaQuyen == pq.MaQuyen && o.MaDM == pq.MaDM && o.DelTime == null);

                    if (qr.Any())
                    {
                        ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                        ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

                        TempData["Error"] = "Ban da phan quyen nay roi";
                        return View(pq);
                    }
                    else
                    {
                        qr = db.PhanQuyens.Where(o => o.MaQuyen == pq.MaQuyen && o.MaDM == pq.MaDM && o.DelTime != null);

                        if (qr.Any())
                        {
                            PhanQuyen pq1 = qr.SingleOrDefault();

                            pq1.MaQuyen = pq.MaQuyen;
                            pq1.MaDM = pq.MaDM;
                            pq1.DelTime = null;

                            db.SubmitChanges();

                            return RedirectToAction("DanhSachPhanQuyen", "PhanQuyen");
                        }
                        else
                        {
                            db.PhanQuyens.InsertOnSubmit(pq);
                            db.SubmitChanges();

                            return RedirectToAction("DanhSachPhanQuyen", "PhanQuyen");
                        }
                    }
                }
                else
                {
                    ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                    ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

                    TempData["Error"] = "Vui long chon day du thong tin de phan quyen";
                    return View(pq);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

                TempData["Error"] = "Không thể phan quyen, chi tiet loi: " + ex;
                return View(pq);
            }
        }

        public ActionResult SuaPhanQuyen(string maquyen, string madm)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
            ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

            PhanQuyen pq = db.PhanQuyens.FirstOrDefault(o => o.MaQuyen == maquyen && o.MaDM == madm && o.DelTime == null);

            return View(pq);
        }

        [HttpPost]
        public ActionResult SuaPhanQuyen(PhanQuyen pq)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                if (!string.IsNullOrEmpty(pq.MaQuyen) && !string.IsNullOrEmpty(pq.MaDM))
                {
                    var qr = db.PhanQuyens.Where(o => o.MaQuyen == pq.MaQuyen && o.MaDM == pq.MaDM && o.DelTime == null);

                    if (qr.Any())
                    {
                        PhanQuyen pq1 = qr.SingleOrDefault();
                        pq1.MaQuyen = pq.MaQuyen;
                        pq1.MaDM = pq.MaDM;

                        db.SubmitChanges();

                        return RedirectToAction("DanhSachPhanQuyen", "PhanQuyen");
                    }
                    else
                    {
                        ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                        ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

                        TempData["Error"] = "Không tim thay phan quyen";
                        return View(pq);
                    }
                }
                else
                {
                    ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                    ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

                    TempData["Error"] = "Không tim thay phan quyen";
                    return View(pq);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Quyen = db.Quyens.Where(o => o.DelTime == null).ToList();
                ViewBag.DM = db.DanhMucs.Where(o => o.DelTime == null).ToList();

                TempData["Error"] = "Không thể cap nhat thong tin phan quyen, chi tiet loi: " + ex;
                return View(pq);
            }
        }

        public ActionResult XoaPhanQuyen(string maquyen, string madm)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            try
            {
                var qr = db.PhanQuyens.Where(o => o.MaQuyen == maquyen && o.MaDM == madm && o.DelTime == null);

                PhanQuyen pq = qr.SingleOrDefault();

                pq.DelTime = DateTime.Now;

                db.SubmitChanges();

                return RedirectToAction("DanhSachPhanQuyen", "PhanQuyen");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DanhSachPhanQuyen", "PhanQuyen");
            }
        }

        [HttpPost]
        public JsonResult TimPhanQuyen(string ttpq)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                var QC = new QuyenController();
                var DMC = new DanhMucController();

                var dspq = (from item in db.PhanQuyens.Where(o => o.DelTime == null)
                            select new
                            {
                                TenQuyen = QC.GetQuyen(item.MaQuyen).TenQuyen,
                                TenDM = DMC.GetDanhMuc(item.MaDM).TenDM,
                                MaQuyen = item.MaQuyen,
                                MaDM = item.MaDM
                            }).ToList();

                if (!string.IsNullOrEmpty(ttpq))
                {
                    if (QC.GetQuyenbyTen(ttpq) != null)
                    {
                        var maquyen = QC.GetQuyenbyTen(ttpq).MaQuyen;

                        dspq = (from item in db.PhanQuyens.Where(o => o.MaQuyen == maquyen && o.DelTime == null)
                                select new
                                {
                                    TenQuyen = QC.GetQuyen(item.MaQuyen).TenQuyen,
                                    TenDM = DMC.GetDanhMuc(item.MaDM).TenDM,
                                    MaQuyen = item.MaQuyen,
                                    MaDM = item.MaDM
                                }).ToList();
                    }
                    else
                    {
                        dspq = null;
                    }    
                }

                return Json(new { dspq = dspq }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}