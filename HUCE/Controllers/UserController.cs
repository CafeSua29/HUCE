using HUCE.App_Start;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HUCE.Controllers
{
    public class UserController : Controller
    {
        public DBConnecterDataContext db = new DBConnecterDataContext();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public User GetUser(string maus)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                RedirectToAction("Login", "Login");
            else
            {
                return db.Users.Where(o => o.MaUser == maus && o.DelTime == null).SingleOrDefault();
            }

            return null;
        }

        public ActionResult ChiTietUser(string maus)
        {
            if (string.IsNullOrEmpty(SessionConfig.GetSession()))
                return RedirectToAction("Login", "Login");

            var User = db.Users.Where(o => o.MaUser == maus && o.DelTime == null).SingleOrDefault();

            return View(User);
        }
    }
}