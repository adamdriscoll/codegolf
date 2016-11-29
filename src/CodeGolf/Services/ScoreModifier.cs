using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Services
{
    public class ScoreModifier
    {
        private const int UpvoteValue = 1;
        private const int DownvoteValue = -1;
        private const int NewProblemValue = 5;
        private const int NewSolutionValue = 1;
        private const int TopSolutionValue = 5;

        private readonly DocumentDbService _service;

        public async Task VoteOnSolution(User solutionAuthor, Vote vote)
        {
            var action = new ScoreModifierAction
            {
                Id = Guid.NewGuid(),
                ModifierType = ModifierType.Vote,
                Modifier = vote.Id,
                Value = vote.Value > 0 ? UpvoteValue : DownvoteValue
            };

            solutionAuthor.Score += action.Value;

            await _service.Repository.Users.Update(solutionAuthor);
        }

        public async Task NewProblem(User problemAuthor, Problem problem)
        {
            var action = new ScoreModifierAction
            {
                Id = Guid.NewGuid(),
                Modifier = problem.Id,
                ModifierType = ModifierType.Problem,
                Value = NewProblemValue
            };

            problemAuthor.Score += action.Value;

            await _service.Repository.Users.Update(problemAuthor);
        }
    }
}
