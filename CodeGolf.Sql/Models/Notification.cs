using System.ComponentModel.DataAnnotations.Schema;

namespace CodeGolf.Sql.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        public User User { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Text { get; set; }

        public bool Seen { get; set; }

        public bool Dismissed { get; set; }
    }

    public enum NotificationType
    {
        Problem,
        SolutionComment,
        Solution
    }
}
