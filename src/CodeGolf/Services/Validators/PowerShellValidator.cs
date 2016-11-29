using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using Newtonsoft.Json;

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

        private async Task<string> WriteTestCases(IEnumerable<Problem.TestCase> testCases, string solution)
        {
            var testFileContent = new StringBuilder();
            var index = 0;
            foreach (var testCase in testCases)
            {
                var solutionContent = new StringBuilder();

                if (!IsEmptyInput(testCase.Input))
                {
                    solutionContent.AppendLine(testCase.Input);
                }

                solutionContent.AppendLine(solution);

                var testCaseContent = FormatTestCase(index, solutionContent.ToString(), testCase.Output);
                testFileContent.AppendLine(testCaseContent);

                index++;
            }

            var solutionId = Language + Guid.NewGuid();

            var solutionFolder = $"/{solutionId}/";
            var solutionFile = $"{solutionFolder}run.tests.ps1";
            var pesterExecutionFile = $"{solutionFolder}run.ps1";
            var testContents = $"Describe 'CodeGolfProblem' {{ {testFileContent} }}";

            await _azureFunctionsService.WriteFile(pesterExecutionFile, FormatPesterExecutionFile(solutionId));
            await _azureFunctionsService.WriteFile(solutionFile, testContents);
            await _azureFunctionsService.WriteFunctionJson(solutionFolder, "res");

            return solutionId;
        }

        private static string FormatPesterExecutionFile(string solutionId)
        {
            return $@" 
                Import-Module D:\home\pester-3.4.3\pester.psd1
                Invoke-Pester -Quiet -Script 'D:\home\site\wwwroot\{solutionId}\*' -OutputXml 'D:\home\site\wwwroot\{solutionId}\TestResults.xml' -ErrorAction SilentlyContinue
                [xml]$xml = Get-Content 'D:\home\site\wwwroot\{solutionId}\TestResults.xml'
                $output = $xml.'test-results'.'test-suite'.results.'test-suite'.results.'test-case' | ForEach-Object {{ [PSCustomObject]@{{name=$_.name;success=$_.success;message=$_.failure.message }}}} 
                $output = ConvertTo-Json $output
                if ($xml.'test-results'.total -eq 1) {{
                    $output = ""[$output]""
                }}
                Out-File -Encoding Ascii -FilePath $res -inputObject $output
            ";
        }

        private static string FormatTestCase(int index, string content, string should)
        {
            if (!should.ToLower().Contains("should"))
            {
                should = $"$output | Should be '{should}'";
            }

            return $@"
                It '{index}' {{
                    function Run 
                    {{
                        {content}
                    }}

                    $output = Run

                    {should}
                }}
            ";
        }

        public async Task<ValidationResult> Validate(Problem problem, string solution)
        {
            var testCaseResults = new List<TestCaseResult>();

            var solutionId = await WriteTestCases(problem.TestCases, solution);
            Thread.Sleep(500);
            var output = await _azureFunctionsService.StartFunction(solutionId);
            
            await _azureFunctionsService.DeleteFunction("/" + solutionId);

            try
            {
                var pesterResults = JsonConvert.DeserializeObject<List<PesterResult>>(JsonConvert.DeserializeObject<string>(output));
                foreach (var pesterResult in pesterResults)
                {
                    testCaseResults.Add(new TestCaseResult(pesterResult.Message, pesterResult.Success == "True"));
                }
            }
            catch
            {
                var pesterResult = JsonConvert.DeserializeObject<PesterResult>(output);
                testCaseResults.Add(new TestCaseResult(pesterResult.Message, pesterResult.Success == "True"));
            }
            
            return new ValidationResult(testCaseResults);
        }
    }

    public class PesterResult
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public string Success { get; set; }
    }
}
