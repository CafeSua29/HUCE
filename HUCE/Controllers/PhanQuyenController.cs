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
            List<PhanQuyen> listpq = db.PhanQuyens.Where(o => o.DelTime == null && o.MaQuyen == maquyen).ToList();

            List<DanhMuc> listdmcha = new List<DanhMuc>();
            List<DanhMuc> listdmcon = new List<DanhMuc>();

            foreach (PhanQuyen pq in listpq)
            {
                var qr = db.DanhMucs.Where(o => o.DelTime == null && o.MaDM == pq.MaDM).SingleOrDefault(); 

                if(!listdmcha.Contains(qr))
                {
                    listdmcha.Add(qr);
                }
            }

            foreach (DanhMuc dm in listdmcha)
            {
                var qr = db.DanhMucs.Where(o => o.DelTime == null && o.MaDMCha == dm.MaDM).ToList();

                foreach (DanhMuc dmc in qr)
                {
                    if (!listdmcon.Contains(dmc))
                    {
                        listdmcon.Add(dmc);
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
    }
}