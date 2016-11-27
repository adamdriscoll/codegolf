using System;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using CodeGolf.Services.Validators;
using NSubstitute;
using NUnit.Framework;

namespace CodeGolf.Test.Services.Validators
{
    [TestFixture]
    public class PowerShellValidatorTest
    {
        private PowerShellValidator _validator;
        private IAzureFunctionsService _azureFunctionsService;

        [SetUp]
        public void Init()
        {
            _azureFunctionsService = Substitute.For<IAzureFunctionsService>();
            _validator = new PowerShellValidator(_azureFunctionsService);
        }

        [Test]
        public void LanguageShouldBePowerShell()
        {
            Assert.AreEqual("PowerShell", _validator.Language);
        }

        [Test]
        public async Task ShouldWritePowerShellFunctionForTestCaseWithSolutionContent()
        {
            var problem = new Problem();
            var testCase = new Problem.TestCase();
            testCase.Input = "input";
            testCase.Output = "output";

            problem.TestCases = new[] {testCase};

            await _validator.Validate(problem, "output");

            await _azureFunctionsService.Received(1)
                .WritePowerShellFunction(Arg.Any<string>(), "input" + Environment.NewLine + "output");
        }

        [Test]
        public async Task ShouldWritePowerShellFunctionForTestCaseWithNaInput()
        {
            var problem = new Problem();
            var testCase = new Problem.TestCase();
            testCase.Input = "N\\A";
            testCase.Output = "output";

            problem.TestCases = new[] { testCase };

            await _validator.Validate(problem, "output");

            await _azureFunctionsService.Received(1)
                .WritePowerShellFunction(Arg.Any<string>(), "output");
        }

        [Test]
        public async Task ShouldWritePowerShellFunctionForTestCaseWithNoneInput()
        {
            var problem = new Problem();
            var testCase = new Problem.TestCase();
            testCase.Input = "None";
            testCase.Output = "output";

            problem.TestCases = new[] { testCase };

            await _validator.Validate(problem, "output");

            await _azureFunctionsService.Received(1)
                .WritePowerShellFunction(Arg.Any<string>(), "output");
        }

        [Test]
        public async Task ShouldWritePowerShellFunctionForTestCaseWithStartsWithNonInput()
        {
            var problem = new Problem();
            var testCase = new Problem.TestCase();
            testCase.Input = "None!!";
            testCase.Output = "output";

            problem.TestCases = new[] { testCase };

            await _validator.Validate(problem, "output");

            await _azureFunctionsService.Received(1)
                .WritePowerShellFunction(Arg.Any<string>(), "output");
        }
    }
}
