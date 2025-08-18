using CollabIn.EF;
using CollabIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CollabIn.Controllers
{
    public class AuthController : Controller
    {
        CollabInEntities1 db = new CollabInEntities1();


        //Main login page
        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel Model)
        {
            if (!ModelState.IsValid)
                return View(Model);

            var PasswordHash = GetMd5Hash(Model.Password);

            if (Model.Usertype == "Member")
            {
                var User = (from u in db.Members
                        where u.Username.Equals(Model.Username) &&
                        u.Password.Equals(PasswordHash)
                        select u).SingleOrDefault();
                if (User != null)
                {
                    Session["user"] = User;
                    Session["UserType"] = "Member";
                    return RedirectToAction("MemberDashboard", "Member");

                }
                
            }
            else if (Model.Usertype == "Supervisor")
            {
                var User = (from u in db.Supervisors
                        where u.Username.Equals(Model.Username) &&
                        u.Password.Equals(PasswordHash)
                        select u).SingleOrDefault();
                if (User != null)
                {
                    Session["User"] = User;
                    Session["UserType"] = "Supervisor";
                    return RedirectToAction("SupervisorDashboard", "Supervisor");
                }
                
            }

            TempData["ErrorMsg"] = "Invalid Username or Password";
            return View(Model);
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Validation failed → return the same model so values stay in fields
                return View(model);
            }

            if (model.UserType == "Member")
            {
                var member = new Member
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Password = GetMd5Hash(model.Password),
                    Email = model.Email,
                    Dob = model.Dob
                };
                db.Members.Add(member);
            }
            else if (model.UserType == "Supervisor")
            {
                var supervisor = new Supervisor
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Password = GetMd5Hash(model.Password),
                    Email = model.Email,
                    Dob = model.Dob
                };
                db.Supervisors.Add(supervisor);
            }

            db.SaveChanges();
            return RedirectToAction("Login");
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        static string GetMd5Hash(string input)
        {
            // Create MD5 instance
            using (MD5 md5 = MD5.Create())
            {
                // Convert input string to byte array
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Compute the MD5 hash
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2"));  // x2 = lowercase hex format

                return sb.ToString();
            }

        }
    }
}