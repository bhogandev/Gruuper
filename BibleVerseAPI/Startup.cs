using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibleVerse.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using BibleVerse.DTO;
using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BibleVerseAPI  
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

            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins("http://localhost:3000", "http://gruuper-stage.herokuapp.com", "http://gruuper.herokuapp.com")
                        ;

                    });

            });
            
            services.AddControllers();
            services.AddScoped<RegistrationRepository>();
            services.AddScoped<ELogRepository>();
            services.AddScoped<BibleVerse.Repositories.UserRepositories.UserActionRepository>();
            services.AddScoped<AWSRepository>();
            services.AddScoped<OrganizationRepository>();
            services.AddScoped<JWTRepository>();
            services.AddScoped<BibleVerse.Exceptions.BVException>();
            services.AddScoped<BibleVerse.Repositories.APIHelperV1>();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<BibleVerse.DALV2.BVIdentityContext>().AddDefaultTokenProviders();
            services.AddDbContext<BibleVerse.DALV2.BVIdentityContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));
            var jwtSection = Configuration.GetSection("JWTSettings");
            services.Configure<JWTSettings>(jwtSection);
            services.AddSignalR();


            var runInTestMode = Configuration.GetValue<string>("RunInTestMode");

            if(runInTestMode == "Y")
            {

            }

            //Validate the token sent by clients
            var appSettings = jwtSection.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "BibleVerseAPI V1");
            });

            
            app.UseHttpsRedirection();

            app.UseRouting();

            
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Hubs.ChatHub>("gruupmessenger");
            });
        }
    }
}
