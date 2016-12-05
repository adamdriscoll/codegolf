using System;
using System.Collections.Generic;

namespace CodeGolf.Sql.Models
{
    public class User 
    {
        public int UserId { get; set; }
        public string Identity { get; set; }
        public string Authentication { get; set; }
        public int Score { get; set; }

        public virtual List<Problem> Problems { get; set; }
        public virtual List<Solution> Solutions { get; set; }
        public virtual List<SolutionComment> SolutionComments { get; set; }
        public virtual List<Vote> Votes { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
