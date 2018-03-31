using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using TallyJ3.Data;
using TallyJ3.Services;
using TallyJ3.Core.Hubs;
using Microsoft.Extensions.FileProviders;
using System.IO;
using TallyJ3.Core.Helper;

namespace TallyJ3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public static IHostingEnvironment Env { get; private set; }
        public static ServiceProvider ServiceProvider { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddSession(services);
            AddDatabase(services);
            AddIdentity(services);

            services.AddSignalR();
            AddSignalrHubs(services);

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();

            ServiceProvider = services.BuildServiceProvider();
        }


        private static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
        }

        private static void AddDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        private static void AddSession(IServiceCollection services)
        {
            services.AddDistributedSqlServerCache(c =>
            {
                c.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
                c.SchemaName = "dbo";
                c.TableName = "AspCoreSessionCache";
            });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Env = env;

            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            UseSignalrHubs(app);

            app.UseStaticFiles();

            // enable use of .js and .css files beside their pages
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Pages")),
                RequestPath = FileLinking.RequestPath
            });

            app.UseAuthentication();

            app.UseMvc();
        }


        private void AddSignalrHubs(IServiceCollection services)
        {
            services.AddTransient<PublicHub>();
        }

        private static void UseSignalrHubs(IApplicationBuilder app)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<PublicHubCore>("/" + PublicHub.HubName);
            });
        }
    }
}
