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
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var passwordHash = GetMd5Hash(model.Password);
            object user = null;
            string redirectController = "";

            if (model.Usertype == "Member")
            {
                user = db.Members
                         .SingleOrDefault(u => u.Username == model.Username && u.Password == passwordHash);
                redirectController = "MemberDashboard";
            }
            else if (model.Usertype == "Supervisor")
            {
                user = db.Supervisors
                         .SingleOrDefault(u => u.Username == model.Username && u.Password == passwordHash);
                redirectController = "SupervisorDashboard";
            }

            if (user == null)
            {
                TempData["ErrorMsg"] = "Invalid Username or Password";
                return View(model);
            }

            // Login successful
            return RedirectToAction("Dashboard", redirectController);
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