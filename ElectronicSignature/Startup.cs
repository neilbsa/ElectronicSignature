using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using ElectronicSignature.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ElectronicSignature.Service.Implementation;
using ElectronicSignature.Service.Interface;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.DataRepositories.Implementation;
using ElectronicSignature.Core.Repository.Core.Interfaces;
using ElectronicSignature.Core.Repository.Core.Implementations;

namespace ElectronicSignature
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


            services.AddScoped<ISystemAuthentication, SystemAuthentication>();
            services.AddScoped<ISignatories, DocumentSignatories>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IElementCoordinates, DocElementCoordinates>();
            services.AddScoped<IFileRepository, FileRepositoryService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IFolderStructureService, FoldereStructureRepository>();
            services.AddScoped<IDocumentFolderLink, DocumentFolderLink>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")

                    ));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                ); ;
            services.AddRazorPages().AddRazorRuntimeCompilation();



            var issuer = Configuration["Issuer"];
            var audience = Configuration["Audience"];
            var sec = Configuration["Secret"];
            bool ValidateIssuer = Boolean.Parse(Configuration["ValidateIssuer"]);
            bool ValidateAudience = Boolean.Parse(Configuration["ValidateAudience"]);
            bool ValidateLifeTime = Boolean.Parse(Configuration["ValidateLifeTime"]);
            bool ValidateIssuerSigningKey = Boolean.Parse(Configuration["ValidateIssuerSigningKey"]);

            var tokenParam = new TokenValidationParameters()
            {
                ValidateIssuer = ValidateIssuer,
                ValidateAudience = ValidateAudience,
                ValidateLifetime = ValidateLifeTime,
                ValidateIssuerSigningKey = ValidateIssuerSigningKey,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(sec))
            };


            services.AddHttpContextAccessor();
            services.AddAuthentication(opt => {
                //opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                options.ReturnUrlParameter = "returnUrl";
                // options.AccessDeniedPath = options.LoginPath;
            })
           .AddJwtBearer(
                opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = tokenParam;
                });







         


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
