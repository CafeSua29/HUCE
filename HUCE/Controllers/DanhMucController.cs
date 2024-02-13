using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class DanhMucController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: DanhMuc
        public ActionResult Index()
        {
            return View();
        }

        public DanhMuc GetDanhMuc(string madm)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.DanhMucs.SingleOrDefault(o => o.MaDM == madm && o.DelTime == null);
            }

            return null;
        }

        public DanhMuc GetDanhMucbyTen(string tendm)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.DanhMucs.FirstOrDefault(o => o.TenDM.Contains(tendm) && o.DelTime == null);
            }

            return null;
        }
    }
}