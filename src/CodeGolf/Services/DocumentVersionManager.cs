using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Services
{
    public class DocumentVersionManager
    {
        private DocumentDbService _dbService;
        private readonly Guid DocumentVersionDocumentId = new Guid("E6A4E557-22CE-4763-B294-5106A60DA5AB");
        private readonly List<IDocumentUpgradeStep> _steps;

        public DocumentVersionManager(DocumentDbService dbService)
        {
            _dbService = dbService;
            _steps = new List<IDocumentUpgradeStep>();
            _steps.Add(new Version1_0());
            _steps.Add(new Version1_1());
        }

        public async Task Upgrade()
        {
            var version = _dbService.GetDocument<DocumentVersion>(DocumentVersionDocumentId);
            if (version == null)
            {
                version = new DocumentVersion();
                version.Version = new Version(1,0);
                await _dbService.CreateDocument(version);
            }

            if (version.Version >= _steps.Max(m => m.Version))
            {
                return;
            }

            foreach (var step in _steps)
            {
                await step.Step(_dbService);
                version.Version = step.Version;
                await _dbService.UpdateDocument(version);
            }
        }
    }

    public class Version1_0 : IDocumentUpgradeStep
    {
        public async Task Step(DocumentDbService dbService)
        {
            await dbService.CreateDocument(new Language { Name = "csharp", DisplayName = "C#" });
            await dbService.CreateDocument(new Language { Name = "powershell", DisplayName = "PowerShell" });
            await dbService.CreateDocument(new Language { Name = "bat", DisplayName = "Batch" });
            await dbService.CreateDocument(new Language { Name = "coffee", DisplayName = "Coffee" });
            await dbService.CreateDocument(new Language { Name = "cpp", DisplayName = "C++" });
            await dbService.CreateDocument(new Language { Name = "fsharp", DisplayName = "F#" });
            await dbService.CreateDocument(new Language { Name = "go", DisplayName = "Go" });
            await dbService.CreateDocument(new Language { Name = "jade", DisplayName = "Jade" });
            await dbService.CreateDocument(new Language { Name = "java", DisplayName = "Java" });
            await dbService.CreateDocument(new Language { Name = "objective-c", DisplayName = "Objective C" });
            await dbService.CreateDocument(new Language { Name = "python", DisplayName = "Python" });
            await dbService.CreateDocument(new Language { Name = "r", DisplayName = "R" });
            await dbService.CreateDocument(new Language { Name = "ruby", DisplayName = "Ruby" });
            await dbService.CreateDocument(new Language { Name = "sql", DisplayName = "SQL" });
            await dbService.CreateDocument(new Language { Name = "swift", DisplayName = "Swift" });
            await dbService.CreateDocument(new Language { Name = "vb", DisplayName = "Visual Basic" });
        }

        public Version Version => new Version(1, 1);
    }

    public class Version1_1 : IDocumentUpgradeStep
    {
        public async Task Step(DocumentDbService dbService)
        {
            var languages = dbService.Client.CreateDocumentQuery<Language>(dbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Language)
                .ToList();

            foreach (var language in languages)
            {
                language.SupportsValidation = language.Name == "powershell";
                await dbService.UpdateDocument(language);
            }
        }

        public Version Version => new Version(1,1);
    }

    public interface IDocumentUpgradeStep
    {
        Task Step(DocumentDbService dbService);
        Version Version { get; }
    }
}
