using System.Collections.Generic;
using System.Linq;
using CodeGolf.Models;
using CodeGolf.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class LanguageController : Controller
    {
        private readonly DocumentDbService _documentDbService;
        public LanguageController(DocumentDbService dbService)
        {
            _documentDbService = dbService;
        }

        public IEnumerable<Language> Get()
        {
            return _documentDbService.Client.CreateDocumentQuery<Language>(_documentDbService.DatabaseUri).Where(m => m.Type == DocumentType.Language);
        }
    }
}