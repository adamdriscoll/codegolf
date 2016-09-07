using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeGolf.Services
{
    public class AzureFunctionsService
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
            var uriBuilder = new UriBuilder(_executionUrl);
            uriBuilder.Path += name;

            var client = new HttpClient();
            return await client.GetStringAsync(uriBuilder.Uri);
        }

        public async Task DeleteFunction(string path)
        {
            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = "api/vfs/site/wwwroot/" + path.Replace("\\", "/");
            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            await client.DeleteAsync(uriBuilder.Uri);
        }

        public async Task WriteCSharpFunction(string path, string content)
        {
            var body = $@"
                using System.Net;
                public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
                {{
                    dynamic input = await req.Content.ReadAsAsync<object>();
                    {content}
                    return req.CreateResponse(HttpStatusCode.OK, output);
                }}
            ";

            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = "api/vfs/site/wwwroot/" + path.Replace("\\", "/") + "run.csx";
            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            await client.PutAsync(uriBuilder.Uri, new StringContent(body));
            await WriteFunctionJson(path);
        }

        public async Task WritePowerShellFunction(string path, string content)
        {
            var body = $@"
                function Run 
                {{
                    {content}
                }}

                $output = Run
                
                Out-File -Encoding Ascii -FilePath $res -inputObject $output
            ";

            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = "api/vfs/site/wwwroot/" + path.Replace("\\", "/") + "run.ps1";
            var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            await client.PutAsync(uriBuilder.Uri, new StringContent(body));
            await WriteFunctionJson(path);
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
