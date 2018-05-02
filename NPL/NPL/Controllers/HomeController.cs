using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;
using PagedList;
using PagedList.Mvc;

namespace NPL.Controllers
{
    public class HomeController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();

        private List<MonAn> LayMonAnMoi(int count)
        {
            return data.MonAns.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult Index()
        {                         
            var monanmoi = LayMonAnMoi(12);
            return View(monanmoi);
        }               
        public ActionResult NhomMonAn (int id)
        {
            var nhommonan = from n in data.Nhoms select n;
            return PartialView(nhommonan);
        }
        public ActionResult Details(int id)
        {
            var monan = from m in data.MonAns
                        where m.IDMonAn == id
                        select m;

            return View(monan.Single());
        }
        public ActionResult Table_ThucDon(int idMonAn)
        {
            List<ThucDon> all = data.ThucDons.Where(i=>i.IDMonAn==idMonAn).ToList();
            return PartialView(all);
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

        public ActionResult MonAnTheoLoai(int? idLoai, int? page)
        {
            if (idLoai == null)
            {
                return RedirectToAction("Index");
            }

            List<MonAn> list = data.MonAns.Where(i => i.IDLoai == idLoai).ToList();
            int pageSize = 6;
            int pageNum = page ?? 1;

            return View(list.ToPagedList(pageNum, pageSize));
        }
    }
}