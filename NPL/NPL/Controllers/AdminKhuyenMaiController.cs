using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;
using PagedList;

namespace NPL.Controllers
{
    public class AdminKhuyenMaiController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();

        public ActionResult Index(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            List<KhuyenMai> all = data.KhuyenMais.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(all.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            int idThucDon = Convert.ToInt32(form["IDThucDon"]);
            double giamGia = Convert.ToDouble(form["GiamGia"]);

            ViewBag.MessageFail = string.Empty;
            if (idThucDon == 0)
            {
                ViewBag.MessageFail += "Không có thực đơn để thêm. ";
            }
            if (giamGia < 0 || giamGia > 100)
            {
                ViewBag.MessageFail += "% Khuyến mại không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View();
            }                     
            
            KhuyenMai khuyenMai = new KhuyenMai();
            khuyenMai.IDThucDon = idThucDon;
            khuyenMai.GiamGia = giamGia;

            if (ModelState.IsValid)
            {
                data.KhuyenMais.InsertOnSubmit(khuyenMai);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY constraint"))
                    {
                        ViewBag.MessageFail = string.Format("Đã tồn tại khuyến mại cho thực đơn {0}.", khuyenMai.ThucDon.TenThucDon);
                    }
                    else
                    {
                        ViewBag.ErrorBody = ex.ToString();
                    }
                    return View();
                }
            }

            ViewBag.MessageSuccess = "Thêm khuyến mại: [" + khuyenMai.ThucDon.TenThucDon + "] thành công";
            return View();
        }
        
        public ActionResult Edit(int? id)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            KhuyenMai khuyenMai = data.KhuyenMais.SingleOrDefault(i => i.IDThucDon == id);
            return View(khuyenMai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
            int idThucDon = Convert.ToInt32(form["IDThucDon"]);
            double giamGia = Convert.ToDouble(form["GiamGia"]);

            KhuyenMai km = data.KhuyenMais.SingleOrDefault(i => i.IDThucDon == idThucDon);

            ViewBag.MessageFail = string.Empty;
            if (idThucDon == 0)
            {
                ViewBag.MessageFail += "Không có thực đơn để thêm. ";
            }
            if (giamGia < 0 || giamGia > 100)
            {
                ViewBag.MessageFail += "% Khuyến mại không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View(km);
            }

            if (ModelState.IsValid)
            {
                UpdateModel(km);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY constraint"))
                    {
                        ViewBag.MessageFail = string.Format("Đã tồn tại khuyến mại cho thực đơn {0}.", km.ThucDon.TenThucDon);
                    }
                    else
                    {
                        ViewBag.ErrorBody = ex.ToString();
                    }
                    return View();
                }
            }

            ViewBag.MessageSuccess = "Đã thay đổi thông tin khuyến mại cho [" + km.ThucDon.TenThucDon + "] thành công";
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            int id = Convert.ToInt32(form["id"]);
            KhuyenMai khuyenMai = data.KhuyenMais.SingleOrDefault(i => i.IDThucDon == id);
            //Nếu nhóm tồn tại
            if (khuyenMai != null)
            {
                //Xóa nhóm
                data.KhuyenMais.DeleteOnSubmit(khuyenMai);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    //int countThucDon = data.ThucDons.Count(i => i.IDMonAn == id);
                    //ViewBag.IsError = true;
                    //if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    //{
                    //    ViewBag.ErrorBody = string.Format("Không thể xóa món ăn [{0}] do có {1} thực đơn trong món ăn này.", monAn.TenMonAn, countThucDon);
                    //}
                    //else
                    //{
                    ViewBag.ErrorBody = ex.ToString();
                    //}
                    List<KhuyenMai> all = data.KhuyenMais.ToList();
                    return View("Index", all);
                }
            }
            //Xóa thành công hoặc nhóm k tồn tại thì trở về Index
            return RedirectToAction("Index", "AdminKhuyenMai");
        }

        [ChildActionOnly]
        public ActionResult PV_Dropdown_ThucDon(int? idThucDon)
        {
            List<ThucDon> all = data.ThucDons.OrderBy(i => i.TenThucDon).ToList();
            if (idThucDon != null)
            {
                ViewBag.idThucDon = idThucDon;
            }
            return PartialView(all);
        }

        [HttpPost]
        public ActionResult ThucDonById(int? val)
        {
            if (val != null)
            {
                //Values are hard coded for demo. you may replae with values 
                // coming from your db/service based on the passed in value ( val.Value)
                int id = val.Value;
                ThucDon thucDon = data.ThucDons.SingleOrDefault(i => i.IDThucDon == id);
                string donGia = Convert.ToDecimal(thucDon.DonGia).ToString("N0") + "đ";
                return Json(new { Success = "true", Data = new { DonGia = donGia } });
            }
            return Json(new { Success = "false" });
        }

        [HttpPost]
        public ActionResult GetGiaKhuyenMai(int? idThucDon, int? val)
        {
            if (val != null && idThucDon != null)
            {
                //Values are hard coded for demo. you may replae with values 
                // coming from your db/service based on the passed in value ( val.Value)
                double percent = Convert.ToDouble(val.Value) / 100;
                int id = idThucDon.Value;
                ThucDon thucDon = data.ThucDons.SingleOrDefault(i => i.IDThucDon == id);
                if (thucDon != null)
                {
                    decimal giaGiam= Convert.ToDecimal(thucDon.DonGia) * Convert.ToDecimal(percent);
                    string giaKhuyenMai = (Convert.ToDecimal(thucDon.DonGia) - giaGiam).ToString("N0") + "đ";
                    return Json(new { Success = "true", Data = new { GiaKhuyenMai = giaKhuyenMai } });
                }
                else
                {
                    return Json(new { Success = "false" });
                }
                
            }
            return Json(new { Success = "false" });
        }
        //end class
    }
}