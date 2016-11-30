using CodeGolf.Interfaces;
using CodeGolf.Models;

namespace CodeGolf.Services.Executors
{
    public class JavaScriptExecutor : Executor
    {
        public JavaScriptExecutor(IAzureFunctionsService azureFunctionsService) : base(azureFunctionsService)
        {
        }

        protected override string FormatCode(string text)
        {
            return $@"
                        var body = '';
                        function o(s) {{
                            body += s;
                        }}

                        module.exports = function (context, req) {{
                                {text}
                                res = {{
                                    body: body
                                }};

                            context.done(null, res);
                        }};
                        ";
        }

        protected override string FileName => "index.js";
        protected override string OutputParameter => "$return";
        public override ICodeGolfLanguage Language => new JavaScriptCodeGolfLanguage();
    }
}