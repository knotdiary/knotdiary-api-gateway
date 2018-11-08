using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using KnotDiary.ApiGateway.Http;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Splunk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnotDiary.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;

            var splunkToken = Configuration.GetValue<string>("Logging:SplunkToken");
            var splunkUrl = Configuration.GetValue<string>("Logging:SplunkCollectorUrl");
            var splunkFormatter = new CompactSplunkJsonFormatter(true, environment.EnvironmentName, "api_log", Environment.MachineName);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .WriteTo.EventCollector(splunkUrl, splunkToken, splunkFormatter)
                .WriteTo.Console()
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
            services.AddSingleton(typeof(HttpAuthServiceSettings), Configuration.GetSection("Services:AuthApi").Get<HttpAuthServiceSettings>());
            services.AddSingleton<IHttpAuthService, HttpAuthService>();

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            services.AddSingleton(serializerSettings);

            // cors setup
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // add authentication
            var authenticationProviderKey = "KnotDiary";
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(authenticationProviderKey, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = Configuration["AuthServiceEndpoint"];
                    options.Audience = "api1";
                });

            // ocelot setup
            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpAuthService httpAuthService, ILogger logger)
        {
            // cors config
            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
                c.AllowCredentials();
            });

            // ocelot config
            var ocelotConfiguration = new OcelotPipelineConfiguration
            {
                PreErrorResponderMiddleware = async (ctx, next) =>
                {
                    await next.Invoke();
                },
                AuthenticationMiddleware = async (ctx, next) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(ctx?.DownstreamReRoute?.AuthenticationOptions?.AuthenticationProviderKey))
                        {
                            await next.Invoke();
                            return;
                        }

                        var token = GetAuthToken(ctx.HttpContext);
                        
                        if (httpAuthService == null)
                        {
                            ctx.HttpContext.Response.StatusCode = 401;
                            await next.Invoke();
                            return;
                        }

                        var user = await httpAuthService.GetUser(token);
                        if (user == null || user.Data == null || !user.IsSuccess)
                        {
                            ctx.HttpContext.Response.StatusCode = 401;
                            await next.Invoke();
                            return;
                        }

                        var userIdString = AttachUserId(ctx.HttpContext, user.Data.Id);
                        if (!string.IsNullOrEmpty(userIdString))
                        {
                            ctx.DownstreamRequest.AbsolutePath = userIdString;
                            ctx.DownstreamRequest.Query = $"userId={user.Data.Id}";
                        }
                        
                        await next.Invoke();
                    }
                    catch (Exception ex)
                    {
                        var errors = ctx.Errors.Select(a => $"{a.Message}; ");
                        logger.Error(ex, $"KnotDiary.ApiGateway.Startup | AuthenticationMiddleware | Authentication has thrown an error | {errors}");
                        ctx.HttpContext.Response.StatusCode = 401;
                        await next.Invoke();
                    }
                }
            };

            app.UseOcelot(ocelotConfiguration).Wait();
        }

        private string ReattachQueryStrings(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }

            var urlPathString = httpContext.Request.Path;

            var queryStrings = httpContext.Request.Query;
            foreach(var queryString in queryStrings)
            {
                urlPathString += QueryHelpers.AddQueryString(urlPathString, new Dictionary<string, string> { { queryString.Key, queryString.Value.FirstOrDefault() } });
            }

            return urlPathString;
        }

        private string AttachUserId(HttpContext httpContext, string id)
        {
            if (httpContext == null)
            {
                return null;
            }
            
            var userIdQueryString = httpContext.Request.Query["userId"].ToString();
            if (string.IsNullOrEmpty(userIdQueryString))
            {
                var queryStringOptions = new Dictionary<string, string> { { "userId", id } };
                var newUri = QueryHelpers.AddQueryString(httpContext.Request.Path, queryStringOptions);
                return newUri;
            }

            return null;
        }

        private string GetAuthToken(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;
            var authHeaders = headers?.Count > 0 ? headers["Authorization"].ToString() : null;

            if (!string.IsNullOrEmpty(authHeaders))
            {
                var token = authHeaders.Replace("Bearer ", "");
                return token;
            }

            return null;
        }
    }
}
