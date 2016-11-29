using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Interfaces.Repository
{
    public interface ICourseRepository
    {
        Task Initialize();
        Task<Course> Get(Guid id);
        Task<Course> Get(string name);
        Task<IEnumerable<Course>> Get();
        Task Create(Course course);
        Task Delete(Course course);
    }
}
