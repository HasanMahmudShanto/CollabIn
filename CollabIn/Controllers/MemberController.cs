using AutoMapper;
using CollabIn.DTOs;
using CollabIn.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CollabIn.Controllers
{
    public class MemberController : Controller
    {
        CollabInEntities1 db = new CollabInEntities1();

        static Mapper GetMapperForDashboard()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Project, ProjectDTO>().ReverseMap();
            });
            return new Mapper(config);
        }
        static Mapper GetMapperForDetails()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Project, ProjectDetailsDTO>().ReverseMap();
            });
            return new Mapper(config);
        }
        public ActionResult MemberDashboard()
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
            if (ProjectsData == null)
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
            var ProjectDetailsData = GetMapperForDetails().Map<ProjectDetailsDTO>(ProjectsData);
            ProjectDetailsData.Name = MemberNames;
            ProjectDetailsData.MemberId = MemberIds;
            ProjectDetailsData.SupervisorName = db.Supervisors
                .Where(s => s.Id == ProjectsData.SupervisorId)
                .Select(s => s.Username)
                .FirstOrDefault();
            return View(ProjectDetailsData);
        }
    }
}