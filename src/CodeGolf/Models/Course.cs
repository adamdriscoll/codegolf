using System;
using System.Collections.Generic;
namespace CodeGolf.Models
{
    public class Course : CodeGolfDocument
    {
        public List<Guid> ProblemIds { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override DocumentType Type { get; }
    }
}
