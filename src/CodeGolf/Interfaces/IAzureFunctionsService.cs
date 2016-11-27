using System.Threading.Tasks;

namespace CodeGolf.Interfaces
{
    public interface IAzureFunctionsService
    {
        Task WriteFunctionJson(string path);
        Task<string> StartFunction(string name);
        Task UploadZip(string url, string name);
        Task DeleteFunction(string path);
        Task WriteCSharpFunction(string path, string content);
        Task WritePowerShellFunction(string path, string content);
    }
}