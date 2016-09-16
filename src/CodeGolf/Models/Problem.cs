using System;
using System.Collections.Generic;

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
        [Obsolete]
        public string Input { get; set; }
        [Obsolete]
        public string Output { get; set; }
        public IEnumerable<TestCase> TestCases { get; set; }
        public Guid[] Solutions { get; set; }
        public int SolutionCount { get; set; }
        public Guid Language { get; set; }
        public Guid Author { get; set; }
        public override DocumentType Type => DocumentType.Problem;

        public class TestCase
        {
            public string Input { get; set; }
            public string Output { get; set; }
        }
    }
}