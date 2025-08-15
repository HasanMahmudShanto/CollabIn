using CollabIn.EF;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CollabIn.DTOs;

namespace CollabIn.Controllers
{
    public class SupervisorController : Controller
    {
        CollabInEntities1 db = new CollabInEntities1();

        static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Project, ProjectDTO>().ReverseMap();
            });
            return new Mapper(config);
        }
        public ActionResult SupervisorDashboard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var Data = db.Projects.ToList();
            var Projects = GetMapper().Map<List<ProjectDTO>>(Data);
            return View(Projects);
        }
        [HttpGet]
        public ActionResult Create() 
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(string Title, DateTime StartDate, DateTime EndDate)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var User = (Supervisor)Session["User"];
            int SupervisorID = User.Id;
            string Status = "Open";
            int Members = 0;

            var NewProject = new Project()
            {
                Title = Title,
                Status = Status,
                Members = Members,
                StartDate = StartDate,
                EndDate = EndDate,
                SupervisorId = SupervisorID,

            };
            db.Projects.Add(NewProject);
            db.SaveChanges();
            TempData["SuccessMsg"] = "New project has been added";

            return View();
        }
    }
}