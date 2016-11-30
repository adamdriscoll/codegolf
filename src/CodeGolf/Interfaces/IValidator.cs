using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Interfaces
{
    public interface IValidator
    {
        ICodeGolfLanguage Language { get; }

        Task<ValidationResult> Validate(Problem problem, string solution);
    }

    public class ValidationResult
    {
        public ValidationResult(IEnumerable<TestCaseResult> testCaseResults)
        {
            TestCaseResults = testCaseResults;
            Passed = TestCaseResults.All(m => m.Passed);
        }

        public bool Passed { get; }

        public IEnumerable<TestCaseResult> TestCaseResults { get; }
    }

    public class TestCaseResult
    {
        public TestCaseResult(string expectedOutput, string actualOutput)
        {
            ExpectedOutput = expectedOutput;
            ActualOutput = actualOutput;
            Passed = expectedOutput.Equals(ActualOutput, StringComparison.OrdinalIgnoreCase);
        }

        public TestCaseResult(string actualOutput, bool passed)
        {
            ActualOutput = actualOutput;
            Passed = passed;
        }

        public string ExpectedOutput { get; }

        public string ActualOutput { get; }

        public bool Passed { get; }
    }

}
