using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;

namespace CodeGolf.Services.Validators
{
    public class CSharpValidator : IValidator
    {
        private readonly AzureFunctionsService _azureFunctionsService;

        public CSharpValidator(AzureFunctionsService azureFunctionsService)
        {
            _azureFunctionsService = azureFunctionsService;
        }

        public ICodeGolfLanguage Language { get; } = new CSharpCodeGolfLanguage();

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

                var solutionId = Language.Name + Guid.NewGuid();
                await _azureFunctionsService.WriteCSharpFunction("/" + solutionId + "/", solutionContent);
                Thread.Sleep(500);
                var output = await _azureFunctionsService.StartFunction(solutionId.ToString());

                output = output.Trim('"').Replace("\\r\\n", Environment.NewLine);

                await _azureFunctionsService.DeleteFunction("/" + solutionId);

                testCaseResults.Add(new TestCaseResult(testCase.Output, output.Trim()));
            }

            return new ValidationResult(testCaseResults);
        }
    }
}
