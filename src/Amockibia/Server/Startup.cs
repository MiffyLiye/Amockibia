using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Amockibia.Utilities;
using Amockibia.Rule;

namespace Amockibia
{
    public class Startup
    {
        private object Locker { get; } = new object();
        private string ServerId { get; }
        private ServerConfig Config { get; }
        public Startup(IHostingEnvironment env)
        {
            ServerId = env.EnvironmentName;
            Config = ServerId.GetConfig();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    var candidateRules = Config.Rules.Where(r => r.Alive() && r.Matches(context.Request)).OrderBy(r => r.Priority).ToList();
                    RequestHandler matchedRule;
                    lock (Locker)
                    {
                        matchedRule = candidateRules.First(r => r.Alive());
                        matchedRule.Reserve();
                    }
                    await matchedRule.Respond(context.Request, context.Response);
                }
                catch (Exception ex)
                {
                    if (context.Response.HasStarted)
                    {
                        throw;
                    }
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(ex.ToString());
                }
            });
        }
    }
}
