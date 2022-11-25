using LogToHtml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lth_Web
{
    public class Startup
    {
        public static string LogFilePath { get; private set; } = string.Empty;

        #region Set options for LogToHtml
        public static Log.Options Options { get => options; private set => options = value; }

        private static Log.Options options = new()
        {
            Project = $"{Assembly.GetCallingAssembly().GetName().Name}",
            LogToConsole = true
        };
        #endregion

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            #region Configure LogToHtml
            List<string> projects = new()
            {
                $"{Options.Project}"
            };
            LogFilePath = Path.Combine(env.WebRootPath, "logging", "loggin.html");
            _ = new Configuration(projects, LogFilePath);
            #endregion

            // Do this in Startup.cs since this will cause a UI lock when the logging file is being created.
            // This is why I'd ideally use threading but currently cannot figure a good way out to handle it.
            if (!File.Exists(LogFilePath))
                Log.Info(options, "Create log file");
        }
    }
}
