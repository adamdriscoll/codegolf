using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Services;
using CodeGolf.Services.Executors;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class DrivingRangeController : Controller
    {
        private readonly ExecutorFactory _executorFactory;

        public DrivingRangeController(AzureFunctionsService azureFunctionsService)
        {
            _executorFactory = new ExecutorFactory(azureFunctionsService);
        }

        [Route("/drivingrange")]
        public IActionResult Index()
        {
            return View(new DrivingRangeViewModel(HttpContext.User.Identity));
        }

        [HttpPost]
        [Route("/drivingrange/execute")]
        public async Task<string> Execute(string content, string language)
        {
            return await _executorFactory.Execute(content, language);
        }

        [Route("/drivingrange/languages")]
        public IEnumerable<string> GetLanguages()
        {
            return _executorFactory.Languages;
        }
    }
}
