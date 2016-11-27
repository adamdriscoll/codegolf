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

    }
}
