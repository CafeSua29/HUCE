using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class QuyenController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: Quyen
        public ActionResult Index()
        {
            return View();
        }

        public Quyen GetQuyen(string maq)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.Quyens.Where(o => o.MaQuyen == maq && o.DelTime == null).SingleOrDefault();
            }

            return null;
        }
    }
}