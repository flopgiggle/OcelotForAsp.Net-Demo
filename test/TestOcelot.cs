using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace test
{
    public class TestOcelot
    {
        public static void StartOcelot()
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddJsonFile("ocelot.json")
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(s =>
                {
                    s.AddOcelot();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {

                })
                .UseIISIntegration()
                .Configure(app =>
                {
                    var configuration = new OcelotPipelineConfiguration
                    {

                        PreErrorResponderMiddleware = async (ctx, next) =>
                        {
                            if (false)
                            {
                                string st111r = "return reson";
                                byte[] array = Encoding.UTF8.GetBytes(st111r);
                                MemoryStream stream = new MemoryStream(array);

                                ctx.HttpContext.Response.StatusCode = 200;
                                //ctx.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "some reason";
                                stream.CopyTo(ctx.HttpContext.Response.Body);
                                ctx.HttpContext.Response.Body.Flush();
                                ctx.HttpContext.Response.Headers.Add("Content-Length", stream.Length.ToString());
                            }
                            await next.Invoke();
                        }
                    };
                    app.UseStaticFiles();
                    //app.UseStaticFiles(new StaticFileOptions()                                      //手工高亮
                    //{                                                                               //手工高亮
                    //    FileProvider = new PhysicalFileProvider(                                    //手工高亮
                    //        Path.Combine(Directory.GetCurrentDirectory(), @"MyStaticFiles")),       //手工高亮
                    //    RequestPath = new PathString("/StaticFiles")                           //手工高亮
                    //});

                    app.UseOcelot(configuration).Wait();
                })
                .UseUrls("http://localhost:5000")
                .Build()
                .Run();
        }
    }
}