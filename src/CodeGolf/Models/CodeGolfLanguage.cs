using System.Collections.Generic;
using System.Linq;

namespace CodeGolf.Models
{
    public interface ICodeGolfLanguage
    {
        string Name { get; }

        string Help { get; }

        bool CanExecute { get; }

        bool CanValidate { get; }
    }

    public class LanguageFactory
    {
        public LanguageFactory()
        {
            Languages = new ICodeGolfLanguage[]
            {
                new BatCodeGolfLanguage(), 
                new CoffeeCodeGolfLanguage(), 
                new CSharpCodeGolfLanguage(), 
                new CppCodeGolfLanguage(), 
                new FSharpCodeGolfLanguage(), 
                new GoCodeGolfLanguage(), 
                new JadeCodeGolfLanguage(), 
                new JavaCodeGolfLanguage(), 
                new JavaScriptCodeGolfLanguage(), 
                new ObjectiveCCodeGolfLanguage(), 
                new OtherCodeGolfLanguage("Other"),
                new PowerShellCodeGolfLanguage(), 
                new PythonCodeGolfLanguage(), 
                new RCodeGolfLanguage(), 
                new RubyCoffeeCodeGolfLanguage(), 
                new SqlCodeGolfLanguage(), 
                new SwiftCodeGolfLanguage(), 
                new VbCodeGolfLanguage()
            };
        }

        public IEnumerable<ICodeGolfLanguage> Languages { get; }

        public ICodeGolfLanguage Get(string name)
        {
            var lang = Languages.FirstOrDefault(m => m.Name == name);
            if (lang == null)
            {
                return new OtherCodeGolfLanguage(name);
            }
            return lang;
        }
    }

    public class OtherCodeGolfLanguage : ICodeGolfLanguage
    {
        public OtherCodeGolfLanguage(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class BatCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Batch";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class CoffeeCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Coffee";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class CppCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "C++";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class CSharpCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "C#";
        public string Help => "Use the Console.Write* to write output to the output window.";
        public bool CanExecute => true;
        public bool CanValidate => true;
    }

    public class FSharpCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "F#";
        public string Help => "Use the Console.Write* to write output to the output window.";
        public bool CanExecute => true;
        public bool CanValidate => false;
    }

    public class GoCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Go";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class JadeCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Coffee";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class JavaCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Java";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class JavaScriptCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "JavaScript";
        public string Help => "Use the o() function to write output to the output window.";
        public bool CanExecute => true;
        public bool CanValidate => false;
    }

    public class ObjectiveCCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Objective C";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class PowerShellCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "PowerShell";
        public string Help => "Output written to the pipeline will appear in the output window.";
        public bool CanExecute => true;
        public bool CanValidate => true;
    }

    public class PythonCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Python";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class RCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "R";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class RubyCoffeeCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Ruby";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class SqlCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Sql";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class SwiftCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Swift";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }

    public class VbCodeGolfLanguage : ICodeGolfLanguage
    {
        public string Name => "Visual Basic";
        public string Help => string.Empty;
        public bool CanExecute => false;
        public bool CanValidate => false;
    }
}
