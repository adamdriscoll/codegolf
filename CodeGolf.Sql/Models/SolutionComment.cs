
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeGolf.Sql.Models
{
    public class SolutionComment 
    {
        public int SolutionCommentId { get; set; }

        public string Comment { get; set; }
        [ForeignKey("Commentor")]
        public int? CommentorId { get; set; }
        public virtual User Commentor { get; set; }
        [ForeignKey("Solution")]
        public int? SolutionId { get; set; }
        public virtual Solution Solution { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
