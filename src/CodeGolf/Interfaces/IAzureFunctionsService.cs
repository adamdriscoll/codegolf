using System.Threading.Tasks;

namespace CodeGolf.Interfaces
{
    public interface IAzureFunctionsService
    {
        Task WriteFunctionJson(string path, string outputParameter);
        Task<string> StartFunction(string name);
        Task UploadZip(string url, string name);
        Task DeleteFunction(string path);
        Task WriteFile(string path, string content);
        Task WriteCSharpFunction(string path, string content);
    }
}