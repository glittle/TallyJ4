using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using TallyJ3.Code.Helper;
using TallyJ3.Code.Hubs;
using TallyJ3.Code.Misc;
using TallyJ3.Data;
using TallyJ3.Services;

namespace TallyJ3
{
    public class Startup
    {
        private static IHubContext<PublicHubCore> _publicHub;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public static IHostingEnvironment Env { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IHubContext<PublicHubCore> GlobalPublicHub
        {
            get
            {
                return _publicHub ?? (_publicHub = ServiceProvider.GetService<IHubContext<PublicHubCore>>());
            }
        }



        // Add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddLogging(services);
            AddSession(services);
            AddDatabase(services);
            AddIdentity(services);
            AddSignalrHubs(services);

            services.AddMemoryCache();

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();


            services.AddSingleton<ILogHelper, LogHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddSingleton<ISharedEnvironment, SharedEnvironment>();

            //ServiceProvider = services.BuildServiceProvider();

            services.AddSingleton<IPublicHubHelper, PublicHubHelper>();
        }


        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Env = env;
            ServiceProvider = serviceProvider;

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


        private static void AddLogging(IServiceCollection services)
        {
            services.AddLogging(builder => builder
                            .AddConsole()
                            .AddDebug()
                            .AddConfiguration(Configuration.GetSection("Logging"))
                        );
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

            services.AddTransient<ApplicationDbContext>();
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



        private void AddSignalrHubs(IServiceCollection services)
        {
            services.AddSignalR();

            // registered here so that they can be injected into PageModels
            services.AddSingleton<PublicHubCore>();
            //services.AddSingleton<IAnalyzeHub, AnalyzeHub>();
            //services.AddSingleton<IMainHub, MainHub>();
            //services.AddTransient<IStatusUpdateHub, IStatusUpdateHub>();
        }

        private static void UseSignalrHubs(IApplicationBuilder app)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<PublicHubCore>("/public");
                //routes.MapHub<AnalyzeHubCore>("/analyze");
                //routes.MapHub<ImportHubCore>("/import");
                //routes.MapHub<MainHubCore>("/main");
                //routes.MapHub<FrontDeskHubCore>("/frontdesk");
                //routes.MapHub<RollCallHubCore>("/rollcall");
            });
        }
    }
}
