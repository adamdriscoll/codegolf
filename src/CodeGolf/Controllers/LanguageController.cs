using System.Collections.Generic;
using CodeGolf.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class LanguageController : Controller
    {
        private readonly LanguageFactory _factory;

        public LanguageController(LanguageFactory factory)
        {
            _factory = factory;
        }

        public IEnumerable<ICodeGolfLanguage> Get()
        {
            return _factory.Languages;
        }
    }
}