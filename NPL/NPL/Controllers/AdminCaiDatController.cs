using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;

namespace NPL.Controllers
{
    public class AdminCaiDatController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();

        // GET: AdminCaiDat
        public ActionResult Index(bool? result)
        {
            List<CaiDat> configs = data.CaiDats.ToList();
            if (result != null)
            { 
                ViewBag.Result = (bool)result ? "Cập nhật thành công." : null;
            }
            return View(configs);
        }

        [HttpPost]
        public ActionResult ApplyConfig(FormCollection form)
        {
            List<CaiDat> configs = data.CaiDats.ToList();
            string validateMsg = string.Empty;

            foreach (CaiDat cf in configs)
            {
                switch (cf.TenThamSo)
                {
                    case "top_thuc_don_take":
                        string input = form["top_thuc_don_take"];
                        if (string.IsNullOrWhiteSpace(input) || !IsDigit(input))
                        {
                            ViewData["Error"+cf.TenThamSo] = "Giá trị không hợp lệ";
                            return View("Index", configs);
                        }
                        else
                        {
                            int iinput = Convert.ToInt32(input);
                            if (iinput < 1 || iinput > 10)
                            {
                                ViewData["Error" + cf.TenThamSo] = "Hãy nhập giá trị trong khoảng từ 1-10";
                                return View("Index", configs);
                            }
                        }
                        break;
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        string tenThamSo = cf.TenThamSo;
                        string giaTri = form[cf.TenThamSo];

                        data.p_UpdateCaiDat(tenThamSo, giaTri);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = ex.ToString();
                        View("Index", configs);
                    }
                }
            }
            
            return RedirectToAction("Index", new { result = true });
        }

        private bool IsDigit(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }










    }
}