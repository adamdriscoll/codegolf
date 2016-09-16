using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Services
{
    public class ProblemValidatorService
    {
        private readonly IEnumerable<IValidator> _validators;

        public ProblemValidatorService(AzureFunctionsService azureFunctionsService)
        {
            _validators = new List<IValidator>
            {
                new PowerShellValidator(azureFunctionsService)
            };
        }

        public async Task<ValidationResult> Validate(string language, Problem problem, string solution)
        {
            var validator = _validators.FirstOrDefault(m => m.Language.Equals(language, StringComparison.OrdinalIgnoreCase));
            if (validator != null)
                return await validator.Validate(problem, solution);

            return null;
        }
    }

    public interface IValidator
    {
        string Language { get; }

        Task<ValidationResult> Validate(Problem problem, string solution);
    }

    public class PowerShellValidator : IValidator
    {
        private readonly AzureFunctionsService _azureFunctionsService;

        public PowerShellValidator(AzureFunctionsService azureFunctionsService)
        {
            _azureFunctionsService = azureFunctionsService;
        }

        public string Language { get; } = "PowerShell";

        public async Task<ValidationResult> Validate(Problem problem, string solution)
        {
            var testCaseResults = new List<TestCaseResult>();
            foreach (var testCase in problem.TestCases)
            {
                string solutionContent = string.Empty;
                if (string.IsNullOrWhiteSpace(testCase.Input) ||
                    testCase.Input.Equals("None", StringComparison.OrdinalIgnoreCase) ||
                    testCase.Input.Equals("N\\A", StringComparison.OrdinalIgnoreCase) ||
                    testCase.Input.StartsWith("None", StringComparison.OrdinalIgnoreCase))
                {

                }
                else
                {
                    solutionContent = testCase.Input;
                    solutionContent += Environment.NewLine;
                }

                solutionContent += solution;

                var solutionId = Guid.NewGuid();
                await _azureFunctionsService.WritePowerShellFunction("/" + solutionId + "/", solutionContent);
                Thread.Sleep(500);
                var output = await _azureFunctionsService.StartFunction(solutionId.ToString());
                await _azureFunctionsService.DeleteFunction("/" + solutionId);

                testCaseResults.Add(new TestCaseResult(testCase.Output, output));
            }
            
            return new ValidationResult(testCaseResults);
        }
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

        public string ExpectedOutput { get; }

        public string ActualOutput { get; }

        public bool Passed { get; }
    }
}
