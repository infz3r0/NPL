using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;

namespace NPL.Controllers
{
    public class HomeController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        [ChildActionOnly]
        public ActionResult PartialNavbarItem()
        {
            List<Nhom> all = data.Nhoms.ToList();
            return PartialView(all);
        }

        [ChildActionOnly]
        public ActionResult PartialDropdownItem(int? idNhom)
        {
            if (idNhom == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Loai> loai = data.Loais.Where(i => i.IDNhom == idNhom).ToList();
            return PartialView(loai);
        }
    }
}