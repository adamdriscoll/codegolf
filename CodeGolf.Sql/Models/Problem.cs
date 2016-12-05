using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeGolf.Sql.Models
{
    public class Problem 
    {
        public int ProblemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<TestCase> TestCases { get; set; }
        public virtual List<Solution> Solutions { get; set; }
        public string Language { get; set; }
        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public virtual User Author { get; set; }
        public bool AnyLanguage { get; set; }
        public bool EnforceOutput { get; set; }
        public bool Closed { get; set; }
        public DateTime DateAdded { get; set; }
    }
}