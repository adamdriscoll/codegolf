using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeGolf.Sql.Models
{
    public class Solution 
    { 
        public int SolutionId { get; set; }
        public string Content { get; set; }
        public string Language { get; set; }
        [ForeignKey("Problem")]
        public int? ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public virtual User Author { get; set; }
        public bool? Passing { get; set; }
        public virtual List<Vote> Votes { get; set; }
        public virtual List<SolutionComment> SolutionComments { get; set; }
        public DateTime DateAdded { get; set; }
    }
}