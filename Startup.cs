using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using AutoMapper;
using GlobalExceptionHandler.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using VueCliMiddleware;
using VueExample.Color;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Helpers;
using VueExample.Hubs;
using VueExample.Providers;
using VueExample.Providers.Abstract;
using VueExample.Providers.ChipVerification;
using VueExample.Providers.ChipVerification.Abstract;
using VueExample.Providers.Srv6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services;
using VueExample.StatisticsCore.Abstract;
using VueExample.StatisticsCore.Services;

namespace VueExample
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
            services.AddControllersWithViews();

            // Add AddRazorPages if the app uses Razor Pages.
            services.AddRazorPages();

            // In production, the Vue files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddResponseCompression(options=>options.EnableForHttps = true);
     
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default60",
                    new CacheProfile()
                    {
                        Duration = 3600
                    });
            });
            services.AddAutoMapper();
            services.AddLazyCache();
            services.AddSignalR();
            services.AddOptions();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "0.0.0.0:1799";
                options.InstanceName = "srvredis";
            });

            services.AddCors(o => o.AddPolicy("DefaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
                
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v0.2.2", new OpenApiInfo
            //     {
            //         Version = "v0.2.2",
            //         Title = "SVR_API",
            //         Description = "SVR_MES_19_API_0.2.2"
            //     });
            //     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //     c.IncludeXmlComments(xmlPath);
            // });
        

            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ApplicationContext")), ServiceLifetime.Transient);
            services.AddDbContext<Srv6Context>(options => options.UseSqlServer(Configuration.GetConnectionString("SRV6Context")), ServiceLifetime.Transient);

            
            
           // services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundTasks.OnlineTestingService>();
           
            services.AddScoped<IUserProvider, UserProvider>();
            services.AddTransient<IWaferMapProvider, WaferMapProvider>();
            services.AddTransient<IDeviceTypeProvider, DeviceTypeProvider>();
            services.AddTransient<IDieProvider, DieProvider>();
            services.AddTransient<IDividerService, DividerService>();
            services.AddTransient<IMeasurementProvider, SimpleMeasurementProvider>();
            services.AddTransient<IGraphicProvider, BasicGraphicProvider>();
            services.AddTransient<ISRV6GraphicService, GraphicService>();
            services.AddTransient<IStandartWaferService, StandartWaferService>();
            services.AddTransient<IUploaderService, UploaderService>();
            services.AddTransient<IDefectProvider, DefectProvider>();
            services.AddTransient<IMeasurementRecordingService, MeasurementRecordingService>();
            services.AddTransient<IPhotoProvider, PhotoProvider>();
            services.AddTransient<IFileGraphicUploaderService, FileGraphicUploaderService>();
            services.AddTransient<IChartJSProvider, ChartJSProvider>();
            services.AddTransient<IDieValueService, DieValueService>();
            services.AddTransient<IColorService, ColorService>();
            services.AddTransient<IGradientService, GradientService>();
            services.AddTransient<IElementService, ElementService>();
            services.AddTransient<IStatParameterService, StatParameterService>();
            services.AddTransient<IElementTypeProvider, ElementTypeProvider>();
            services.AddTransient<IElementTypeService, ElementTypeService>();
            services.AddTransient<ISpecificElementTypeProvider, SpecificElementTypeProvider>();
            services.AddTransient<ISpecificElementTypeService, SpecificElementTypeService>();
            services.AddTransient<IFolderService, FolderService>();
            services.AddTransient<IDieTypeProvider, DieTypeProvider>();
            services.AddTransient<IWaferProvider, WaferProvider>();
            services.AddTransient<IStandartWaferProvider, StandartWaferProvider>();
            services.AddTransient<ICodeProductProvider, CodeProductProvider>();
            services.AddTransient<IProcessProvider, ProcessProvider>();
            services.AddTransient<IDeviceProvider, DeviceProvider>();
            services.AddTransient<IStageProvider, StageProvider>();
            services.AddTransient<IMeasurementSetProvider, MeasurementSetProvider>();
            services.AddTransient<IAtomicMeasurementProvider, AtomicMeasurementProvider>();
            services.AddTransient<IMaterialProvider, MaterialProvider>();
            services.AddTransient<IFacilityProvider, FacilityProvider>();
            services.AddTransient<IMeasuredDeviceProvider, MeasuredDeviceProvider>();
            services.AddTransient<IPointProvider, PointProvider>();
            services.AddTransient<IExportProvider, ExportService>();
            services.AddTransient<IShortLinkProvider, ShortLinkProvider>();
            services.AddTransient<IStandartParameterProvider, StandartParameterProvider>();
            services.AddTransient<IStandartParameterService, StandartParameterService>();
            services.AddTransient<IStandartPatternProvider, StandartPatternProvider>();
            services.AddTransient<IStandartPatternService, StandartPatternService>();
            services.AddTransient<IStandartMeasurementPatternProvider, StandartMeasurementPatternProvider>();
            services.AddTransient<IStandartMeasurementPatternService, StandartMeasurementPatternService>();
            services.AddTransient<IKurbatovParameterBordersProvider, KurbatovParameterBordersProvider>();
            services.AddTransient<IKurbatovParameterBordersService, KurbatovParameterBordersService>();
            services.AddTransient<IKurbatovParameterProvider, KurbatovParameterProvider>();
            services.AddTransient<IKurbatovParameterService, KurbatovParameterService>();
            
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseGlobalExceptionHandler(x => {
                x.ContentType = "application/json";
                x.ResponseBody(s => JsonConvert.SerializeObject(new
                {
                    Message = "An error occurred while processing your request"
                }));
                x.Map<RecordNotFoundException>().ToStatusCode(StatusCodes.Status404NotFound);
                x.Map<CollectionIsEmptyException>().ToStatusCode(StatusCodes.Status404NotFound);
                x.Map<ValidationErrorException>().ToStatusCode(StatusCodes.Status403Forbidden);
            });           


            app.UseCors("DefaultPolicy");
            // app.UseSwagger();
            // app.UseSwaggerUI(c =>
            // {
            //     c.SwaggerEndpoint("v0.2.2/swagger.json", "SVR_MES_19_API_0.2.2");
            //     c.RoutePrefix = string.Empty;
            // });


            app.UseAuthentication();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<LivePointHub>("/livepoint");

                if (env.IsDevelopment())
                {
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "ClientApp" },
                        npmScript: "serve",
                        regex: "Compiled successfully");
                }

                // Add MapRazorPages if the app uses Razor Pages. Since Endpoint Routing includes support for many frameworks, adding Razor Pages is now opt -in.
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            });
        }
    }
}