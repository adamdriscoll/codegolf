using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeGolf.Services.Executors
{
    public class ExecutorFactory
    {
        private readonly IEnumerable<Executor> _executors;

        public ExecutorFactory(AzureFunctionsService azureFunctionsService)
        {
            _executors = new Executor[]
            {
                new PowerShellExecutor(azureFunctionsService),
                new CSharpExecutor(azureFunctionsService),
            };
        }

        public async Task<string> Execute(string text, string language)
        {
            var executor = _executors.FirstOrDefault(m => m.Language == language);
            if (executor == null) throw new Exception($"Executor for {language} not found!");

            return await executor.Execute(text);
        }

        public IEnumerable<string> Languages => _executors.Select(m => m.Language);
    }

    public abstract class Executor
    {
        private readonly AzureFunctionsService _azureFunctionsService;
        protected Executor(AzureFunctionsService azureFunctionsService)
        {
            _azureFunctionsService = azureFunctionsService;
        }

        protected abstract string FileName { get; }

        public abstract string Language { get; }

        protected virtual string FormatCode(string text)
        {
            return text;
        }

        protected virtual string FormatOutput(string output)
        {
            return output;
        }

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
                await _azureFunctionsService.WriteFunctionJson(executionFolder);
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

    public class PowerShellExecutor : Executor
    {
        public PowerShellExecutor(AzureFunctionsService azureFunctionsService) : base(azureFunctionsService)
        {
        }

        protected override string FormatCode(string text)
        {
            return $@"function Run
                    {{
                        {text}
                    }}

                    $output = Run
                    Out-File -Encoding Ascii -FilePath $res -inputObject $output
                    ";
        }

        protected override string FormatOutput(string output)
        {
            return JsonConvert.DeserializeObject<string>(output);
        }

        protected override string FileName => "run.ps1";
        public override string Language => "PowerShell";
    }

    public class CSharpExecutor : Executor
    {
        public CSharpExecutor(AzureFunctionsService azureFunctionsService) : base(azureFunctionsService)
        {
        }

        protected override string FormatCode(string text)
        {
            return $@"
                using System.Net;
                using System.Threading.Tasks;
                using System.Text;
                using System.Linq;
                using System.IO;

                public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
                {{
                    StringBuilder sb = new StringBuilder();
                    StringWriter sw = new StringWriter(sb);
                    Console.SetOut(sw);

                    var output = string.Empty;
                    try 
                    {{
                        {text}

                        output = sb.ToString();
                    }}
                    catch (Exception ex) 
                    {{
                        output = ex.Message;
                    }}

                    return req.CreateResponse(HttpStatusCode.OK, output);
                }}
                    ";
        }

        protected override string FormatOutput(string output)
        {
            return JsonConvert.DeserializeObject<string>(output);
        }

        protected override string FileName => "run.csx";
        public override string Language => "C#";
    }
}
