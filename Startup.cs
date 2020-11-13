using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nebula.CI.Plugins.Common.Result
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(option =>
            {
                option.AllowAnyHeader();
                option.AllowAnyMethod();
                option.AllowAnyOrigin();
            });
            app.UseDefaultFiles();
            app.UseStaticFiles("/api/ci/plugins/common/result");
            
            app.UseRouting();

            var nfsPath = "/nfs";
            
            var fileServerOptions =  new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(nfsPath),
                RequestPath = "/api/ci/plugins/common/result/filebrowser",
                EnableDirectoryBrowsing = true
            };
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileServerOptions.StaticFileOptions.DefaultContentType = MediaTypeNames.Application.Octet;
            app.UseFileServer(fileServerOptions);

            var fileDownloadOptions =  new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(nfsPath),
                RequestPath = "/api/ci/plugins/common/result/filedownload",
                EnableDirectoryBrowsing = true
            };
            fileDownloadOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileDownloadOptions.StaticFileOptions.DefaultContentType = MediaTypeNames.Application.Octet;
            fileDownloadOptions.StaticFileOptions.OnPrepareResponse = new Action<StaticFileResponseContext>( s => {
                s.Context.Response.Headers.Add("Content-Disposition", "attachment");
            });
            app.UseFileServer(fileDownloadOptions);
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa=>{});
        }
    }
}
