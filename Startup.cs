
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using AdviceFirstApi.Domain;
using AdviceFirstApi.Mappings;
using AdviceFirstApi.Persistance;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using FluentValidation;
using FluentValidation.AspNetCore;
using AdviceFirstApi.Filters;

namespace AdviceFirstApi
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IConfiguration Configuration { get; }
        readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            })
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "http://bnz.co.nz/modelvalidationproblem",
                        Title = "One or more model validation occured",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "See the error properties for details.",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            }
            );
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.WithOrigins("*.*"));
            });
            services.AddMvcCore(setupAction: option =>
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add<ValidationFilter>();
            }).AddApiExplorer()
                                 .AddDataAnnotations()
                                 .AddFluentValidation(mvcConfig => mvcConfig.RegisterValidatorsFromAssemblyContaining<Startup>())
                                 //  .AddJsonOptions()
                                 .AddFormatterMappings();
            services.AddDirectoryBrowser();
            var conn = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AdviceFirstDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IAdviceFirstRepository, AdviceFirstRepository>();
            //services.AddAutoMappervices(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(Startup));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("AdviceFirstAPi_V1", new OpenApiInfo { Title = "AdviceFirst API", Version = "v1" });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlFullFileName = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlFullFileName);
                });

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseRouting();
            app.UseHttpsRedirection();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseStaticFiles();
                app.UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/AdviceFirstAPi_V1/swagger.json", "AdviceFirst API V1");
               });
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexcepted fault happened. Try again");
                    });
                });
            }
            //app.UseCors(options=>options.WithOrigins("*"));
            app.UseCors("AllowOrigin *.*");
            app.UseEndpoints(endPoints =>
            {
                endPoints.MapControllers();
            });
            //Logger
        }
    }
}

