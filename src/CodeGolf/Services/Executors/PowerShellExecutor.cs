﻿using CodeGolf.Interfaces;

namespace CodeGolf.Services.Executors
{
    public class PowerShellExecutor : Executor
    {
        public PowerShellExecutor(IAzureFunctionsService azureFunctionsService) : base(azureFunctionsService)
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

        public override IExecutorLanguage Language => new PowerShellExecutorLanguage();
        protected override string FileName => "run.ps1";
        
    }
}