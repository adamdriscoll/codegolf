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
        private readonly string _url;
        private readonly string _username;
        private readonly string _password;
        private readonly string _executionUrl;

        public AzureFunctionsService(string url, string username, string password, string executionUrl)
        {
            _url = url;
            _username = username;
            _password = password;
            _executionUrl = executionUrl;
        }

        public async Task WriteFunctionJson(string path, string outParameter = "res")
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
                Name = outParameter,
                Direction = "out"
            });

            var output = JsonConvert.SerializeObject(function);

            var client = new HttpClient();
            SetClientAuthorization(client);

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
                var executionTask = client.GetStringAsync(uriBuilder.Uri);

                if (await Task.WhenAny(executionTask, Task.Delay(new TimeSpan(0, 0, 15))) == executionTask)
                {
                    return executionTask.Result;
                }

                throw new TimeoutException();
            }
            catch (HttpRequestException ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        public async Task UploadZip(string url, string name)
        {
            var client = new HttpClient();
            SetClientAuthorization(client);

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
            SetClientAuthorization(client);

            var response = await client.SendAsync(request);
            var data = response.Content.ReadAsStringAsync().Result;
        }

        public async Task WriteFile(string path, string content)
        {
            var uriBuilder = new UriBuilder(_url);
            uriBuilder.Path = "api/vfs/site/wwwroot/" + path.Replace("\\", "/");
            var client = new HttpClient();
            SetClientAuthorization(client);
            await client.PutAsync(uriBuilder.Uri, new StringContent(content));
        }

        private void SetClientAuthorization(HttpClient client)
        {
            var byteArray = Encoding.ASCII.GetBytes(_username + ":" + _password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(byteArray));
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
