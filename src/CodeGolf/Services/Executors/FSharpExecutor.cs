using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;

namespace CodeGolf.Services.Executors
{
    public class FSharpExecutor : Executor
    {
        public FSharpExecutor(IAzureFunctionsService azureFunctionsService) : base(azureFunctionsService)
        {
        }

        protected override string FormatCode(string text)
        {
            return $@"
    #r ""System.Net.Http""

    open System.IO
    open System.Text
    open System.Net
    open System.Net.Http

    let Run(req: HttpRequestMessage, log: TraceWriter) =
            let sb = new StringBuilder();
            let sw = new StringWriter(sb);
            Console.SetOut(sw);
            {text}

            req.CreateResponse(HttpStatusCode.OK, sb.ToString());
            ";
        }

        protected override string FileName => "run.fsx";
        public override ICodeGolfLanguage Language => new FSharpCodeGolfLanguage();
    }
}
