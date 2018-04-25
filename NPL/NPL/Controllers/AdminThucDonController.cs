using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;
using PagedList;

namespace NPL.Controllers
{
    public class AdminThucDonController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();
        
        public ActionResult Index(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<ThucDon> all = data.ThucDons.ToList();

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
        public ActionResult Create(FormCollection form, HttpPostedFileBase HinhAnh)
        {
            string tenThucDon = form["TenThucDon"];
            decimal donGia = Convert.ToDecimal(form["DonGia"]);
            int idMonAn = Convert.ToInt32(form["IDMonAn"]);

            ViewBag.MessageFail = string.Empty;
            if (string.IsNullOrWhiteSpace(tenThucDon))
            {
                ViewBag.MessageFail += "Tên thực đơn không hợp lệ. ";
            }
            if (Convert.ToInt32(donGia) % 500 != 0)
            {
                ViewBag.MessageFail += "Đơn giá không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View();
            }

            string _FileName = "";
            try
            {
                if (HinhAnh.ContentLength > 0)
                {
                    _FileName = Path.GetFileName(HinhAnh.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/ThucDon/"), _FileName);
                    if (!System.IO.File.Exists(_path))
                    {
                        HinhAnh.SaveAs(_path);
                    }
                }
            }
            catch
            {
            }

            ThucDon thucDon = new ThucDon();
            thucDon.TenThucDon = tenThucDon;
            thucDon.HinhAnh = _FileName;
            thucDon.DonGia = donGia;
            thucDon.SoLuongDaDat = 0;
            thucDon.IDMonAn = idMonAn;

            if (ModelState.IsValid)
            {
                data.ThucDons.InsertOnSubmit(thucDon);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Thêm thực đơn [" + tenThucDon + "] thành công";
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
            ThucDon thucDon = data.ThucDons.SingleOrDefault(i => i.IDThucDon == id);
            return View(thucDon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form,  HttpPostedFileBase fileUpload)
        {
            string tenThucDon = form["TenThucDon"];
            decimal donGia = Convert.ToDecimal(form["DonGia"]);
            int idThucDon = Convert.ToInt32(form["IDThucDon"]);

            ThucDon thucDon = data.ThucDons.SingleOrDefault(i => i.IDThucDon == idThucDon);

            ViewBag.MessageFail = string.Empty;
            if (string.IsNullOrWhiteSpace(tenThucDon))
            {
                ViewBag.MessageFail += "Tên thực đơn không hợp lệ. ";
            }
            if (Convert.ToInt32(donGia) % 500 != 0)
            {
                ViewBag.MessageFail += "Đơn giá không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View();
            }

            try
            {
                if (fileUpload.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileUpload.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/ThucDon/"), _FileName);
                    if (!System.IO.File.Exists(_path))
                    {
                        fileUpload.SaveAs(_path);
                    }
                    thucDon.HinhAnh = _FileName;
                }
            }
            catch
            {
            }

            if (ModelState.IsValid)
            {
                UpdateModel(thucDon);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Đã thay đổi thông tin thực đơn [" + tenThucDon + "] thành công";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ThucDon thucDon = data.ThucDons.SingleOrDefault(i => i.IDThucDon == id);
            return View(thucDon);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form, int? page)
        {
            int id = Convert.ToInt32(form["id"]);
            ThucDon thucDon = data.ThucDons.SingleOrDefault(i => i.IDThucDon == id);
            //Nếu nhóm tồn tại
            if (thucDon != null)
            {
                if (ModelState.IsValid)
                {
                    //Xóa nhóm
                    data.ThucDons.DeleteOnSubmit(thucDon);
                    try
                    {
                        data.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        int countChiTietDatHang = data.ChiTietDatHangs.Count(i => i.IDThucDon == id);
                        ViewBag.IsError = true;
                        if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        {
                            ViewBag.ErrorBody = string.Format("Không thể xóa thực đơn [{0}] do đã có đơn đặt hàng đặt thực đơn này.", thucDon.TenThucDon, countChiTietDatHang);
                        }
                        else
                        {
                            ViewBag.ErrorBody = ex.ToString();
                        }
                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        List<ThucDon> all = data.ThucDons.ToList();
                        return View("Index", all.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            //Xóa thành công hoặc nhóm k tồn tại thì trở về Index
            return RedirectToAction("Index", "AdminThucDon");
        }

        [ChildActionOnly]
        public ActionResult PV_Dropdown_MonAn(int? idMonAn)
        {
            List<MonAn> all = data.MonAns.OrderBy(i => i.TenMonAn).ToList();
            if (idMonAn != null)
            {
                ViewBag.idMonAn = idMonAn;
            }
            return PartialView(all);
        }
        //end class
    }
}