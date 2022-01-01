using IdentityDemoAPI.Data;
using IdentityDemoAPI.Interfaces;
using IdentityDemoAPI.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityDemoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { private set;  get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<BackgroundServices.TimedHostedServiceTest>();

            //for return json with include ef-core
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityDemoAPI", Version = "v1" });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>( options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AppDbContextConnection"));
            });

            //Identity configuration
            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<AppRole>()
                    .AddRoleManager<RoleManager<AppRole>>()
                    .AddUserManager<UserManager<AppUser>>()
                    .AddSignInManager<SignInManager<AppUser>>()
                    .AddEntityFrameworkStores<AppDbContext>();

            //v1 - Authentication cookies for claimsPrincipal
            //v2 - Using JWT Bearer
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    /*.AddCookie(options =>
                    {
                        options.Events.OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        };
                        options.Events.OnRedirectToAccessDenied = (context) =>
                        {
                            context.Response.StatusCode = 403;
                            return Task.CompletedTask;
                        };
                        options.SlidingExpiration = true;
                        options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
                    })*/

                    .AddJwtBearer(options => 
                    {
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                            ValidIssuer = Configuration["JWT:Issuer"],
                            ValidAudience = Configuration["JWT:Audience"],
                            ClockSkew = TimeSpan.Zero
                        };
                    })
                    .AddGoogle(options =>
                    {
                        var googleAuthSection = Configuration.GetSection("ExternalAuth:GoogleAuth");
                        options.ClientId = googleAuthSection["ClientId"];
                        options.ClientSecret = googleAuthSection["ClientSecret"];
                        options.CallbackPath = "/login-google";
                    })
                    .AddFacebook(options =>
                    {
                        var facebookAuthSection = Configuration.GetSection("ExternalAuth:FacebookAuth");
                        options.AppId = facebookAuthSection["AppId"];
                        options.AppSecret = facebookAuthSection["SecretKey"];
                        options.CallbackPath = "/login-facebook";
                    });
            
            //services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityDemoAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
