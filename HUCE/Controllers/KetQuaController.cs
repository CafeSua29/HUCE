using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class KetQuaController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: KetQua
        public ActionResult Index()
        {
            return View();
        }

        public List<KetQua> GetKetQua(string masv, string mahk)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.KetQuas.Where(o => o.MaSV == masv && o.MaHK == mahk && o.DelTime == null).ToList();
            }

            return null;
        }
    }
}