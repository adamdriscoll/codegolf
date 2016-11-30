using CodeGolf.Interfaces;
using CodeGolf.Models;

namespace CodeGolf.Services.Executors
{
    public class CSharpExecutor : Executor
    {
        public CSharpExecutor(IAzureFunctionsService azureFunctionsService) : base(azureFunctionsService)
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

        public override ICodeGolfLanguage Language => new CSharpCodeGolfLanguage();
        protected override string FileName => "run.csx";
        
    }
}