using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using Newtonsoft.Json;

namespace CodeGolf.Services
{
    public class AzureFunctionsService : IAzureFunctionsService
    {
        private string _url;
        private string _username;
        private string _password;
        private string _executionUrl;

        public AzureFunctionsService(string url, string username, string password, string executionUrl)
        {
            _url = url;
            _username = username;
            _password = password;
            _executionUrl = executionUrl;
        }

        public async Task WriteFunctionJson(string path)
        {
            var function = new Function();
            function.Bindings.Add(new HttpTriggerBinding
            {
                Name = "req",
                Direction = "in",
                AuthLevel = "anonymous"
            });

            function.Bindings.Add(new HttpBinding
            {
                Name = "res",
                Direction = "out"
            });

            var output = JsonConvert.SerializeObject(function);

            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = "api/vfs/site/wwwroot/" + path.Replace("\\", "/") + "/function.json";

            await client.PutAsync(uriBuilder.Uri, new StringContent(output));
        }

        public async Task<string> StartFunction(string name)
        {
            try
            {
                var uriBuilder = new UriBuilder(_executionUrl);
                uriBuilder.Path += name;

                var client = new HttpClient();
                return await client.GetStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        public async Task UploadZip(string url, string name)
        {
            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = $"api/zip/";

            WebClient wc = new WebClient();
            var data = wc.DownloadData(new Uri(url));

            var result = await client.PutAsync(uriBuilder.Uri, new ByteArrayContent(data));
        }

        public async Task DeleteFunction(string path)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_url + "/api/vfs/site/wwwroot/" + path.Replace("\\", "/") + "/?recursive=true"),
                Method = HttpMethod.Delete,
            };

            request.Headers.IfMatch.Add(EntityTagHeaderValue.Any);
            
            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.SendAsync(request);
            var data = response.Content.ReadAsStringAsync().Result;
        }

        public async Task WriteCSharpFunction(string path, string content)
        {
            var body = $@"
                using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

public class Solution {{
    private StringBuilder sb;
    public Solution()
    {{
        sb = new StringBuilder();
    }}

    private void o(string text)
    {{
        sb.AppendLine(text);
    }}

    public void Run()
    {{
        {content}
    }}

    public override string ToString()
    {{
        return sb.ToString();
    }}
}}

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{{
    var solution = new Solution();
    solution.Run();

    return req.CreateResponse(HttpStatusCode.OK, solution.ToString());
}}

            ";

            await WriteFile(path.Replace("\\", "/") + "run.csx", body);
            await WriteFunctionJson(path);
        }

        public async Task WriteFile(string path, string content)
        {
            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = "api/vfs/site/wwwroot/" + path.Replace("\\", "/");
            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            await client.PutAsync(uriBuilder.Uri, new StringContent(content));
        }
    }

    public class Function
    {
        public Function()
        {
            Bindings = new List<Binding>();
        }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }
        [JsonProperty("bindings")]
        public List<Binding> Bindings { get; set; }
    }

    public class Binding
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("direction")]
        public string Direction { get; set; }
    }

    public class HttpTriggerBinding : Binding
    {
        [JsonProperty("authLevel")]
        public string AuthLevel { get; set; }

        public HttpTriggerBinding()
        {
            Type = "httpTrigger";
        }
    }

    public class HttpBinding : Binding
    {
        public HttpBinding()
        {
            Type = "http";
        }
    }
}
