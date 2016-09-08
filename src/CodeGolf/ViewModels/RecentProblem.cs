using System;

namespace CodeGolf.ViewModels
{
    public class RecentProblem 
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public int ShortestSolution { get; set; }

        public int SolutionCount { get; set; }

        public string Author { get; set; }
        public string AuthorId { get; set; }
    }
}