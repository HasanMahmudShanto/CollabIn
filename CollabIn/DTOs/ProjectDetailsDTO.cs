using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollabIn.DTOs
{
    public class ProjectDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int Members { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuperVisorId { get; set; }
        public string Details { get; set; }
        public List<string> Name { get; set; }
        public List<int> MemberId { get; set; }
        public string SupervisorName { get; set; }
    }
}