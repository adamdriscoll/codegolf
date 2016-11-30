using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using CodeGolf.Services.Validators;

namespace CodeGolf.Services
{
    public class ProblemValidatorService
    {
        private readonly IEnumerable<IValidator> _validators;

        public ProblemValidatorService(AzureFunctionsService azureFunctionsService)
        {
            _validators = new List<IValidator>
            {
                new PowerShellValidator(azureFunctionsService),
                new CSharpValidator(azureFunctionsService)
            };
        }

        public async Task<ValidationResult> Validate(string language, Problem problem, string solution)
        {
            var validator = _validators.FirstOrDefault(m => m.Language.Name.Equals(language, StringComparison.OrdinalIgnoreCase));
            if (validator != null)
                return await validator.Validate(problem, solution);

            return null;
        }
    }




}
