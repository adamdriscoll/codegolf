using CodeGolf.Models;
using CodeGolf.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using React.AspNet;

namespace CodeGolf
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();

            services.AddMvc();

            services.AddOptions();

            services.AddAuthentication(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
#if DEBUG
            var database = Configuration.GetValue<string>("DocumentDbConfig:DebugDocumentDb");
#else
            var database = Configuration.GetValue<string>("DocumentDbConfig:DocumentDb");
#endif
            var collection = Configuration.GetValue<string>("DocumentDbConfig:DocumentCollection");
            var endpoint = Configuration.GetValue<string>("DocumentDbConfig:EndpointUri");
            var primaryKey = Configuration.GetValue<string>("DocumentDbConfig:PrimaryKey");

            var dbService = new DocumentDbService(new DocumentDbConfig
            {
                Database = database,
                DocumentCollection = collection,
                EndpointUri = endpoint,
                PrimaryKey = primaryKey
            });

            dbService.EnsureInitialized().Wait();
            var documentVersionManager = new DocumentVersionManager(dbService);
            documentVersionManager.Upgrade().Wait();

            services.AddTransient(x => new DocumentDbService(new DocumentDbConfig
            {
                Database = database,
                DocumentCollection = collection,
                EndpointUri = endpoint,
                PrimaryKey = primaryKey
            }));

            ConfigureAzureFunctionService(services);
            
            services.AddTransient<ProblemValidatorService>();
        }

        public void ConfigureAzureFunctionService(IServiceCollection services)
        {
            var url = Configuration.GetValue<string>("AzureFunctionApi:Url");
            var username = Configuration.GetValue<string>("AzureFunctionApi:Username");
            var password = Configuration.GetValue<string>("AzureFunctionApi:Password");
            var executionUrl = Configuration.GetValue<string>("AzureFunctionApi:ExecutionUrl");

            services.AddTransient(x => new AzureFunctionsService(url, username, password, executionUrl));

            //var service = new AzureFunctionsService(url, username, password, executionUrl);
            //service.UploadZip("https://github.com/pester/Pester/archive/3.4.3.zip", "pester").Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseReact(config =>
            {
                // If you want to use server-side rendering of React components,
                // add all the necessary JavaScript files here. This includes
                // your components as well as all of their dependencies.
                // See http://reactjs.net/ for more information. Example:
                //config
                //  .AddScript("~/Scripts/First.jsx")
                //  .AddScript("~/Scripts/Second.jsx");

                // If you use an external build too (for example, Babel, Webpack,
                // Browserify or Gulp), you can improve performance by disabling
                // ReactJS.NET's version of Babel and loading the pre-transpiled
                // scripts. Example:
                //config
                //  .SetLoadBabel(false)
                //  .AddScriptWithoutTransform("~/Scripts/bundle.server.js");
            });

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = new PathString("/signin"),
                LogoutPath = new PathString("/signout")
            });

#if DEBUG
            var clientId = Configuration.GetValue<string>("GitHubConfig:DebugClientId");
            var clientSecret = Configuration.GetValue<string>("GitHubConfig:DebugClientSecret");
#else
            var clientId = Configuration.GetValue<string>("GitHubConfig:ClientId");
            var clientSecret = Configuration.GetValue<string>("GitHubConfig:ClientSecret");
#endif
            app.UseGitHubAuthentication(options => {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Problem",
                    template: "Problem/View/{problemName}",
                    defaults: new
                    {
                        controller = "Problem",
                        action = "Single",
                    }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
