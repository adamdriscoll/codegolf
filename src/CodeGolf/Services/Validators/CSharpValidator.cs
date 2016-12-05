using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using CodeGolf.Services.Executors;

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

        public async Task<ValidationResult> Validate(Sql.Models.Problem problem, string solution)
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

                var csharpExectuor = new CSharpExecutor(_azureFunctionsService);
                var output=  await csharpExectuor.Execute(solutionContent);

                testCaseResults.Add(new TestCaseResult(testCase.Output, output.Trim()));
            }

            return new ValidationResult(testCaseResults);
        }
    }
}
