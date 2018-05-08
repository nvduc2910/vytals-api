using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vytals.Models;
using Vytals.Options;
using Vytals.Providers;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Vytals.Repository;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Vytals.Hubs;
using Microsoft.AspNetCore.SignalR.Configuration;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Vytals.CustomAttribute;
using System.Reflection;
using AutoMapper;

using Vytals.Infrastructures.Mappings;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Design;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Vytals.Infrastructures;
using Vytals.Helpers;

namespace Vytals
{
    public class Startup 
    {
        private readonly SecurityKey securityKey;
        private readonly IConfigurationSection jwtAppSettingOptions;
        private IMapperConfigurationExpression iMappger;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("SecurityKey").Value));
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddMvc();

            services.AddMvc(options =>
            {
                options.OutputFormatters.Clear();
                options.OutputFormatters.Add(new JsonOutputFormatter(new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }, ArrayPool<char>.Shared));
            });


            //For Localization

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
            

            services.AddScoped<LanguageActionFilter>();

            services.Configure<RequestLocalizationOptions>( options =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("da-DK"),
                    };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });


            services.AddCors();

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

           

            services.AddIdentity<ApplicationUser, MyRole>()
            .AddEntityFrameworkStores<VfDbContext>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();


            services.AddAuthentication(o =>
            {
                //o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(cfg =>
            {
                
                cfg.RequireHttpsMetadata = false;

                cfg.SaveToken = true;

             
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

            });



      
            services.AddSingleton<IFacebookProvider, FacebookProvider>();
            services.AddSingleton<IGooglePlusProvider, GooglePlusProvider>();
          
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Vytals",
                    Description = "CODUX",
                    Contact = new Contact()
                    { Name = "Duc Nguyen", Email = "nvduc2910@gmail.com", }
                });
               
                //c.IncludeXmlComments(GetXmlCommentsPath(PlatformServices.Default.Application));
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = string.Empty;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            //services.AddLogging(builder =>
            //{
            //    builder.AddConfiguration(Configuration.GetSection("Logging"))
            //        .AddConsole()
            //        .AddDebug();
            //});


            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<VfDbContext>(options => options.UseSqlServer(connectionString));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<JwtIssuerOptions>(options =>
            {
                
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            });

            Mapper.Initialize((IMapperConfigurationExpression obj) =>
            {
               
            });

            var maperConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                var types = Assembly.GetExecutingAssembly().GetExportedTypes();
                LoadStandardMappings(types);
                LoadCustomMappings(types, cfg);

            });

            var mapper = maperConfig.CreateMapper();
            services.AddSingleton(mapper);


        }

        public void LoadStandardMappings(IEnumerable<Type> types)
        {
            

            var maps = (from t in types

                        from i in t.GetInterfaces()
                        where i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                        !t.IsAbstract &&
                        !t.IsInterface

                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t
                        }).ToArray();

            foreach (var map in maps)
            {
                Mapper.Map(map.Source, map.Destination);
            }
        }

        public void LoadCustomMappings(IEnumerable<Type> types, IMapperConfigurationExpression configuration)
        {

            var maps = (from t in types

                        from i in t.GetInterfaces()
                        where typeof(IHaveCustomMappings).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        !t.IsInterface
                        select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(configuration);
            }
        }

     
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseCors(
               builder => builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials())
               .UseStaticFiles();
            app.UseWebSockets();


            app.UseAuthentication();

            app.UseSignalR();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }
            app.UseStaticFiles();



            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VF_v1");
                c.InjectOnCompleteJavaScript("/auth.js");
            });

            app.UseMvc();
        }

        public VfDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<VfDbContext>();
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new VfDbContext(builder.Options);
        }
    }

    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "access token",
                    Required = true,
                    Type = "string"
                });
            }
        }
    }

   
}
