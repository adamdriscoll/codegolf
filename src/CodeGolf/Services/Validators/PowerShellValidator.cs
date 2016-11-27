using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;

namespace CodeGolf.Services.Validators
{
    public class PowerShellValidator : IValidator
    {
        private readonly IAzureFunctionsService _azureFunctionsService;

        public PowerShellValidator(IAzureFunctionsService azureFunctionsService)
        {
            _azureFunctionsService = azureFunctionsService;
        }

        public string Language { get; } = "PowerShell";

        private static bool IsEmptyInput(string input)
        {
            return string.IsNullOrWhiteSpace(input) ||
                input.Equals("None", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("N\\A", StringComparison.OrdinalIgnoreCase) ||
                input.StartsWith("None", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> WriteTestCase(Problem.TestCase testCase, string solution)
        {
            var solutionContent = string.Empty;
            if (!IsEmptyInput(testCase.Input))
            {
                solutionContent = testCase.Input;
                solutionContent += Environment.NewLine;
            }

            solutionContent += solution;

            var solutionId = Language + Guid.NewGuid();
            await _azureFunctionsService.WritePowerShellFunction("/" + solutionId + "/", solutionContent);

            return solutionId;
        }

        public async Task<ValidationResult> Validate(Problem problem, string solution)
        {
            var testCaseResults = new List<TestCaseResult>();
            foreach (var testCase in problem.TestCases)
            {
                var solutionId = await WriteTestCase(testCase, solution);
                Thread.Sleep(500);
                var output = await _azureFunctionsService.StartFunction(solutionId);
                await _azureFunctionsService.DeleteFunction("/" + solutionId);

                testCaseResults.Add(new TestCaseResult(testCase.Output, output.Trim()));
            }

            return new ValidationResult(testCaseResults);
        }
    }
}
