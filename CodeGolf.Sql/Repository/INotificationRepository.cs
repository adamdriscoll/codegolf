using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface INotificationRepository
    {
        Task Create(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForUser(int userId);
    }
}
