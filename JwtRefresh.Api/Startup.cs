using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JwtRefresh.Repositories;
using JwtRefresh.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;

namespace JwtRefresh.Api
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Secret"])),
                        ValidateIssuerSigningKey = true,

                        ValidAudience = Configuration["Auth:Audience"],
                        ValidateAudience = true,

                        ValidIssuer = Configuration["Auth:Issuer"],
                        ValidateIssuer = true,

                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero,
                    };
                });

            var conventionPack = new ConventionPack();
            conventionPack.Add(new IgnoreExtraElementsConvention(true));
            conventionPack.Add(new EnumRepresentationConvention(BsonType.String));
            conventionPack.Add(new SnakeCaseElementNameConvention());
            ConventionRegistry.Register("default", conventionPack, t => t.FullName.StartsWith("JwtRefresh."));

            services.AddScoped(provider => new MongoClient(Configuration.GetConnectionString("Default")));
            services.AddScoped(provider => provider.GetService<MongoClient>().GetDatabase(Configuration["Database:Name"]));

            services.AddRepositories();
            services.AddServices();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy(),
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
