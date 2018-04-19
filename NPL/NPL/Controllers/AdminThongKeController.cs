using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;
using PagedList;

namespace NPL.Controllers
{
    public class AdminThongKeController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();
                
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThucDon(int? page)
        {
            List<MonthYear> distinct = data.V_ThongKeThucDons.Select(i => new MonthYear((int)i.Thang, (int)i.Nam)).Distinct().ToList();
            distinct.Reverse();

            int pageSize = 12;
            int pageNum = page ?? 1;
            return View(distinct.ToPagedList(pageNum, pageSize));
        }

        [ChildActionOnly]
        //Dùng trong Index
        public ActionResult PV_ThongKeThucDon()
        {
            int month = 1;
            int year = 2018;

            ViewBag.Month = month;
            ViewBag.Year = year;


            int take = 5;
            ViewBag.Take = take;

            List<V_ThongKeThucDon> all = data.V_ThongKeThucDons.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongSoLuong).Take(take).ToList();

            string list_labels = "[";
            foreach (V_ThongKeThucDon item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_labels += "'" + item.TenThucDon + "'";
                }
                else
                {
                    list_labels += "'" + item.TenThucDon + "'" + ",";
                }
            }
            list_labels += "]";
            ViewBag.List_labels = list_labels;

            string list_data = "[";
            foreach (V_ThongKeThucDon item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_data += "" + item.TongSoLuong + "";
                }
                else
                {
                    list_data += "" + item.TongSoLuong + "" + ",";
                }
            }
            list_data += "]";
            ViewBag.List_data = list_data;

            return PartialView(all);
        }

        [ChildActionOnly]
        //Dùng trong ThucDon
        public ActionResult PV_ThucDonByMonthYear(int month, int year)
        {
            List<V_ThongKeThucDon> all = data.V_ThongKeThucDons.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongSoLuong).ToList();
            return PartialView(all);
        }

        public ActionResult TaiKhoan(int? page)
        {
            List<MonthYear> distinct = data.V_ThongKeUsers.Select(i => new MonthYear((int)i.Thang, (int)i.Nam)).Distinct().ToList();
            distinct.Reverse();

            int pageSize = 12;
            int pageNum = page ?? 1;
            return View(distinct.ToPagedList(pageNum, pageSize));
        }

        [ChildActionOnly]
        //Dùng trong Index
        public ActionResult PV_ThongKeUser()
        {
            int month = 7;
            int year = 2019;

            ViewBag.Month = month;
            ViewBag.Year = year;

            int take = 5;
            ViewBag.Take = take;

            List<V_ThongKeUser> all = data.V_ThongKeUsers.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongThanhTien).Take(take).ToList();

            string list_labels = "[";
            foreach (V_ThongKeUser item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_labels += "'" + item.Username + "'";
                }
                else
                {
                    list_labels += "'" + item.Username + "'" + ",";
                }
            }
            list_labels += "]";
            ViewBag.List_labels = list_labels;

            string list_data_TongThanhTien = "[";
            foreach (V_ThongKeUser item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_data_TongThanhTien += "" + item.TongThanhTien + "";
                }
                else
                {
                    list_data_TongThanhTien += "" + item.TongThanhTien + "" + ",";
                }
            }
            list_data_TongThanhTien += "]";
            ViewBag.List_data_TongThanhTien = list_data_TongThanhTien;
            
            return PartialView(all);
        }

        [ChildActionOnly]
        //Dùng trong TaiKhoan
        public ActionResult PV_TaiKhoanByMonthYear(int month, int year)
        {
            List<V_ThongKeUser> all = data.V_ThongKeUsers.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongThanhTien).ToList();
            return PartialView(all);
        }

    }
}