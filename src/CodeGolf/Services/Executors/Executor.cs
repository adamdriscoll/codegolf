using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using Newtonsoft.Json;

namespace CodeGolf.Services.Executors
{
    public class ExecutorFactory
    {
        private readonly IEnumerable<Executor> _executors;

        public ExecutorFactory(IAzureFunctionsService azureFunctionsService)
        {
            _executors = new Executor[]
            {
                new PowerShellExecutor(azureFunctionsService),
                new CSharpExecutor(azureFunctionsService),
                new JavaScriptExecutor(azureFunctionsService), 
                new FSharpExecutor(azureFunctionsService), 
            };
        }

        public async Task<string> Execute(string text, string language)
        {
            var executor = _executors.FirstOrDefault(m => m.Language.Name == language);
            if (executor == null) throw new Exception($"Executor for {language} not found!");

            return await executor.Execute(text);
        }

        public IEnumerable<ICodeGolfLanguage> Languages => _executors.Select(m => m.Language);
    }

    public abstract class Executor
    {
        private readonly IAzureFunctionsService _azureFunctionsService;
        protected Executor(IAzureFunctionsService azureFunctionsService)
        {
            _azureFunctionsService = azureFunctionsService;
        }

        protected abstract string FileName { get; }

        public abstract ICodeGolfLanguage Language { get; }

        protected virtual string FormatCode(string text)
        {
            return text;
        }

        protected virtual string FormatOutput(string output)
        {
            return JsonConvert.DeserializeObject<string>(output);
        }

        protected virtual string OutputParameter => "res";
        

        public async Task<string> Execute(string text)
        {
            var id = "E" + Guid.NewGuid();
            var executionFolder = $"/{id}/";
            var executionFile = $"{executionFolder}{FileName}";

            string output;
            try
            {
                text = FormatCode(text);
                await _azureFunctionsService.WriteFile(executionFile, text);
                await _azureFunctionsService.WriteFunctionJson(executionFolder, OutputParameter);
                Thread.Sleep(500);
                output = await _azureFunctionsService.StartFunction(id);
                output = FormatOutput(output);
            }
            catch (TimeoutException)
            {
                output = "Function evaluation timed out.";
            }
            finally
            {
                await _azureFunctionsService.DeleteFunction(executionFolder);
            }

            return output;
        }
    }
}
