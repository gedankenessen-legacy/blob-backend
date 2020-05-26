using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Blob_API.AuthModel;
using Blob_API.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation;
using System;

namespace Blob_API
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
            // Datenbank Kontext anlegen
            services.AddDbContext<BlobContext>(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseMySql(Configuration.GetConnectionString("BlobContext"));
            });

            // Auth Datenbank Kontext anlegen
            services.AddDbContext<BlobAuthContext>(opt =>
            {
                opt.UseMySql(Configuration.GetConnectionString("BlobContext"))
                .UseOpenIddict();
            });

            // ASP.NET Core Identity registrieren.
            services.AddIdentity<User, UserRole>()
                .AddEntityFrameworkStores<BlobAuthContext>();

            // OpenIddict hinzufügen und konf.
            services.AddOpenIddict()
                .AddCore(opt =>
                {
                    opt.UseEntityFrameworkCore()
                        .UseDbContext<BlobAuthContext>()
                        .ReplaceDefaultEntities<uint>();
                })
                .AddServer(opt =>
                {
                    opt.UseMvc();
                    opt.DisableHttpsRequirement();
                    opt.EnableTokenEndpoint("/token");
                    opt.AllowPasswordFlow();
                    opt.AcceptAnonymousClients();
                })
                .AddValidation();

            // Identity und OpenIddict nutzen nun die gleichen Claim-Namen.
            services.Configure<IdentityOptions>(opt =>
            {
                opt.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                opt.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                opt.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            // OpenIddict auth schema.
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = OpenIddictValidationDefaults.AuthenticationScheme;
            });

            // Identity konfigurieren.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;

                // Password settings.
                options.Password.RequireDigit = Convert.ToBoolean(Configuration["Identity:RequireDigit"]);
                options.Password.RequireLowercase = Convert.ToBoolean(Configuration["Identity:RequireLowercase"]);
                options.Password.RequireNonAlphanumeric = Convert.ToBoolean(Configuration["Identity:RequireNonAlphanumeric"]);
                options.Password.RequireUppercase = Convert.ToBoolean(Configuration["Identity:RequireUppercase"]);
                options.Password.RequiredLength = Convert.ToInt32(Configuration["Identity:RequiredLength"]);
                options.Password.RequiredUniqueChars = Convert.ToInt32(Configuration["Identity:RequiredUniqueChars"]);

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(Configuration["Identity:DefaultLockoutTimeSpanInMinutes"]));
                options.Lockout.MaxFailedAccessAttempts = Convert.ToInt32(Configuration["Identity:MaxFailedAccessAttempts"]);
                options.Lockout.AllowedForNewUsers = Convert.ToBoolean(Configuration["Identity:AllowedForNewUsers"]);

                // User settings.
                options.User.AllowedUserNameCharacters = Configuration["Identity:AllowedChars"];
                options.User.RequireUniqueEmail = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("ng",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithOrigins(Configuration.GetSection("AllowedHosts").Get<string[]>());
                });
            });

            // NewtonsoftJson nutzen und konfigurieren
            services.AddControllers()
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // Automapper wird dazu verwendent, um unsere DTO-Object zu einem Ressource-Objct zu mappen ohne uns selbst um die zuordnung der Properties zu kümmern.
            services.AddAutoMapper(typeof(Startup));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blob API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blob API v1");
            });

            app.UseRouting();

            app.UseCors("ng");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
