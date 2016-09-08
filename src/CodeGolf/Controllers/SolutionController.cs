using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class SolutionController : Controller
    {
        private readonly DocumentDbService _documentDbService;
        private readonly AzureFunctionsService _azureFunctionsService;

        public SolutionController(DocumentDbService dbService, AzureFunctionsService azureFunctionsService)
        {
            _documentDbService = dbService;
            _azureFunctionsService = azureFunctionsService;
        }

        [HttpGet]
        public Solution Get(Guid id)
        {
            return _documentDbService.GetDocument<Solution>(id);
        }

        [HttpGet]
        public string Raw(Guid id)
        {
            return _documentDbService.GetDocument<Solution>(id).Content;
        }

        [Authorize]
        public async Task<IActionResult> DeleteAsync(Guid guid)
        {
            var solution = _documentDbService.GetDocument<Solution>(guid);
            if (solution == null)
                throw new Exception("Solution not found!");

            var user = _documentDbService.Client.CreateDocumentQuery<User>(_documentDbService.DatabaseUri).Where(m => m.Identity == this.HttpContext.User.Identity.Name && m.Authentication == this.HttpContext.User.Identity.AuthenticationType).ToList().FirstOrDefault();

            if (user == null)
                throw new Exception("User not found!");

            if (solution.Author != user.Id)
                throw new Exception("User does not own solution!");

            var problem = _documentDbService.GetDocument<Problem>(solution.Problem);
            if (problem == null)
                throw new Exception("Problem not found!");

            var solutions = problem.Solutions.ToList();
            solutions.Remove(guid);
            problem.Solutions = solutions.ToArray();
            problem.SolutionCount--;
            await _documentDbService.UpdateDocument(problem);
            await _documentDbService.DeleteDocument(solution.Id);

            return Redirect("/Problem/Index/" + problem.Id);
        }

        [Authorize]
        public async Task<SolutionValidationResult> ValidateAsync(Guid problem, string content)
        {
            var theProblem = _documentDbService.GetDocument<Problem>(problem);
            var language = _documentDbService.GetDocument<Language>(theProblem.Language, true);

            var validation = new SolutionValidationResult();

            //TODO: Clean this up
            if (language.Name == "powershell")
            {
                string solutionContent = string.Empty;
                if (string.IsNullOrWhiteSpace(theProblem.Input) ||
                    theProblem.Input.Equals("None", StringComparison.OrdinalIgnoreCase) ||
                    theProblem.Input.Equals("N\\A", StringComparison.OrdinalIgnoreCase) ||
                    theProblem.Input.StartsWith("None", StringComparison.OrdinalIgnoreCase))
                {
                    
                }
                else
                {
                    solutionContent = theProblem.Input;
                    solutionContent += Environment.NewLine;
                }
                
                solutionContent += content;

                var solutionId = Guid.NewGuid();

                await _azureFunctionsService.WritePowerShellFunction("/" + solutionId + "/", solutionContent);
                Thread.Sleep(500);
                var output = await _azureFunctionsService.StartFunction(solutionId.ToString());
                await _azureFunctionsService.DeleteFunction("/" + solutionId);

                validation.Output = output;
                validation.Succeeded = string.Equals(output.Trim(), theProblem.Output);
            }

            return validation;
        } 

        [Authorize]
        public async Task<IActionResult> PostAsync(Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (string.IsNullOrWhiteSpace(solution.Content))
                throw new Exception("Content is required for solution.");

            var user = _documentDbService.Client.CreateDocumentQuery<User>(_documentDbService.DatabaseUri).Where(m => m.Identity == this.HttpContext.User.Identity.Name && m.Authentication == this.HttpContext.User.Identity.AuthenticationType).ToList().FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Identity = this.HttpContext.User.Identity.Name,
                    Authentication = this.HttpContext.User.Identity.AuthenticationType
                };

                await _documentDbService.CreateDocument(user);
            }

            solution.Author = user.Id;

            await _documentDbService.CreateDocument(solution);
            var problem = _documentDbService.GetDocument<Problem>(solution.Problem);
            var list = problem.Solutions.ToList();
            list.Add(solution.Id);
            problem.Solutions = list.ToArray();
            problem.SolutionCount = problem.Solutions.Length;

            await _documentDbService.Client.UpsertDocumentAsync(_documentDbService.DatabaseUri, problem);

            return Redirect("/Problem/Index/" + problem.Id);
        }
    }
}
