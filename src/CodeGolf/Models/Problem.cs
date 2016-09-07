using System;
using CodeGolf.Api.Models;

namespace CodeGolf.Models
{
    public class Problem : CodeGolfDocument
    {
        public Problem()
        {
            Solutions = new Guid[0];
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public Guid[] Solutions { get; set; }
        public int SolutionCount { get; set; }
        public Guid Language { get; set; }
        public Guid Author { get; set; }
        public override DocumentType Type => DocumentType.Problem;


    }
}