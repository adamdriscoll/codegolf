
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeGolf.Sql.Models
{
    public class TestCase
    {
        public int TestCaseId { get; set; }
        [ForeignKey("Problem")]
        public int? ProblemId { get; set; }
        public virtual Problem Problem { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}
