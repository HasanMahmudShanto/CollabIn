using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollabIn.DTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int Members { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuperVisorId { get; set; }
    }
}