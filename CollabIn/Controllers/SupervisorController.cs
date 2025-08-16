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

        static Mapper GetMapperForDetails()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Project, ProjectDetailsDTO>().ReverseMap();
            });
            return new Mapper(config);
        }
        static Mapper GetMapperForDashboard()
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
            var Projects = GetMapperForDashboard().Map<List<ProjectDTO>>(Data);
            return View(Projects);
        }
        public ActionResult ProjectDetail(int Id)
        {
            var ProjectsData = db.Projects.FirstOrDefault(p => p.Id == Id);
         
            var MemberIds = db.ProjectMembers
                .Where(pm => pm.ProjectId == Id)
                .Select(pm => pm.MemberId)
                .ToList();
            var MemberNames = db.Members
                .Where(m => MemberIds.Contains(m.Id))
                .Select(m => m.Username) 
                .ToList();
            var ProjectDetailsData = GetMapperForDetails().Map<ProjectDetailsDTO>(ProjectsData);
            ProjectDetailsData.Name = MemberNames;
            ProjectDetailsData.MemberId = MemberIds;

            return View(ProjectDetailsData);
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
        [HttpGet]
        public ActionResult Edit(int Id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var ProjectData = db.Projects.FirstOrDefault(p => p.Id == Id);
            if (ProjectData == null)
            {
                return HttpNotFound();
            }
            var MemberIds = db.ProjectMembers
                .Where(pm => pm.ProjectId == Id)
                .Select(pm => pm.MemberId)
                .ToList();
            var MemberNames = db.Members
                .Where(m => MemberIds.Contains(m.Id))
                .Select(m => m.Username)
                .ToList();
            var ProjectDetails = GetMapperForDetails().Map<ProjectDetailsDTO>(ProjectData);
            ProjectDetails.Name = MemberNames;
            ProjectDetails.MemberId = MemberIds;
            return View(ProjectDetails);
        }
        [HttpPost]
        public ActionResult Edit(int Id, string Status, string Title, 
            DateTime StartDate, DateTime EndDate, string Details)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var ProjectData = db.Projects.FirstOrDefault(p => p.Id == Id);
            if (ProjectData == null)
            {
                return HttpNotFound();
            }
            ProjectData.Title = Title;
            ProjectData.StartDate = StartDate;
            ProjectData.EndDate = EndDate;
            ProjectData.Status = Status;
            ProjectData.Details = Details;
            db.SaveChanges();
            TempData["SuccessMsg"] = "Project details updated successfully.";
            return RedirectToAction("ProjectDetail", new { Id = Id });
        }
        public ActionResult RemoveMember(int ProjectId, int MemberId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var ProjectMember = db.ProjectMembers
                .FirstOrDefault(pm => pm.ProjectId == ProjectId && pm.MemberId == MemberId);
            if (ProjectMember != null)
            {
                db.ProjectMembers.Remove(ProjectMember);
                db.SaveChanges();
                TempData["SuccessMsg"] = "Member removed from project successfully.";
            }
            return RedirectToAction("ProjectDetail", new { Id = ProjectId });
        }
    }
}