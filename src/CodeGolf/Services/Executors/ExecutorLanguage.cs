namespace CodeGolf.Services.Executors
{
    public interface IExecutorLanguage
    {
        string Name { get; }

        string Help { get; }
    }

    public class CSharpExecutorLanguage : IExecutorLanguage
    {
        public string Name => "C#";
        public string Help => "Use the Console.Write* to write output to the output window.";
    }

    public class JavaScriptExecutorLanguage : IExecutorLanguage
    {
        public string Name => "JavaScript";
        public string Help => "Use the o() function to write output to the output window.";
    }

    public class PowerShellExecutorLanguage : IExecutorLanguage
    {
        public string Name => "PowerShell";
        public string Help => "Output written to the pipeline will appear in the output window.";
    }
}
