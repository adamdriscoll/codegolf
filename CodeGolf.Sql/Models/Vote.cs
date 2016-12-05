
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeGolf.Sql.Models
{
    public class Vote 
    {
        public int VoteId { get; set; }
        public int ItemId { get; set; }

        [ForeignKey("Voter")]
        public int? VoterId { get; set; }
        public User Voter { get; set; }

        public int Value { get; set; }
    }
}
