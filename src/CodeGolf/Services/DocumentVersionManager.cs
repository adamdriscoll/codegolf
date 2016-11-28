using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;
using Microsoft.Azure.Documents.Client;

namespace CodeGolf.Services
{
    public class DocumentVersionManager
    {
        private readonly DocumentDbService _dbService;
        private readonly List<IDocumentUpgradeStep> _steps;

        public DocumentVersionManager(DocumentDbService dbService)
        {
            _dbService = dbService;
            _steps = new List<IDocumentUpgradeStep>();
            _steps.Add(new Version1_0());
            _steps.Add(new Version1_1());
            _steps.Add(new Version1_2());
            _steps.Add(new Version1_3());
            _steps.Add(new Version1_4());
            _steps.Add(new Version1_5());
        }

        public async Task Upgrade()
        {
            DocumentVersion version;
            try
            {
                version =
                    _dbService.Client.CreateDocumentQuery<DocumentVersion>(_dbService.DatabaseUri)
                        .Where(m => m.Type == DocumentType.Version).ToList().FirstOrDefault();
            }
            catch
            {
                //This is due to an issue in production. Will remove later.
                version = new DocumentVersion();
                version.Version = "1.1";
                await _dbService.CreateDocument(version);
            }

            if (version == null)
            {
                version = new DocumentVersion();
                version.Version = "0.1";
                await _dbService.CreateDocument(version);
            }

            foreach (var step in _steps)
            {
                if (Version.Parse(version.Version) >= step.Version)
                {
                    continue;
                }

                await step.Step(_dbService);
                version.Version = step.Version.ToString();

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

        public Version Version => new Version(1, 0);
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

    public class Version1_2 : IDocumentUpgradeStep
    {
        public async Task Step(DocumentDbService dbService)
        {
            var problems = dbService.GetDocumentType<Problem>(DocumentType.Problem);
            foreach(var problem in problems)
            {
                problem.TestCases = new List<Problem.TestCase>
                {
                    new Problem.TestCase
                    {
                        Input = problem.Input,
                        Output = problem.Output
                    }
                };

                await dbService.UpdateDocument(problem);
            }
        }

        public Version Version => new Version(1, 2);
    }

    /// <summary>
    /// Fixes an issue where we have 2 versions of each language.
    /// </summary>
    public class Version1_3 : IDocumentUpgradeStep
    {
        public async Task Step(DocumentDbService dbService)
        {
            var languages = dbService.GetDocumentType<Language>(DocumentType.Language);
            foreach (var language in languages.OrderBy(m => m.DateAdded).GroupBy(m => m.Name))
            {
                //If we only have one language, don't delete anything
                if (language.Count() == 1) continue;

                var problems = dbService.GetDocumentType<Problem>(DocumentType.Problem);
                foreach (var problem in problems.Where(m => m.Language == language.Last().Id))
                {
                    problem.Language = language.First().Id;
                    await dbService.UpdateDocument(problem);
                }

                await dbService.DeleteDocument(language.Last().Id);
            }
        }

        public Version Version => new Version(1, 3);
    }

    public class Version1_4 : IDocumentUpgradeStep
    {
        public async Task Step(DocumentDbService dbService)
        {
            var language = dbService.Client.CreateDocumentQuery<Language>(dbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Language && m.Name == "csharp")
                .ToList().FirstOrDefault();

            if (language == null)
                throw new Exception("CSharp language not found!");

            language.SupportsValidation = true;
            await dbService.UpdateDocument(language);
        }

        public Version Version => new Version(1, 4);
    }

    public class Version1_5 : IDocumentUpgradeStep
    {
        public async Task Step(DocumentDbService dbService)
        {
            var users = dbService.Client.CreateDocumentQuery<User>(dbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.User)
                .ToList();

            foreach (var user in users)
            {
                await dbService.Repository.Users.Create(user);
                await dbService.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("CodeGolfDB", "CodeGolfCollection", user.Id.ToString()));
            }
        }

        public Version Version => new Version(1, 5);
    }

    public interface IDocumentUpgradeStep
    {
        Task Step(DocumentDbService dbService);
        Version Version { get; }
    }
}
