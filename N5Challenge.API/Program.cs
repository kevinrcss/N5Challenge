using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using N5Challenge.API.Extensions;
using N5Challenge.Application.Interfaces;
using N5Challenge.Application.Services;
using N5Challenge.Core.Interfaces;
using N5Challenge.Infrastructure.Data;
using N5Challenge.Infrastructure.Kafka;
using N5Challenge.Infrastructure.Repositories;
using N5Challenge.Infrastructure.Services;
using N5Challenge.Infrastructure.Settings;
using Serilog;

namespace N5Challenge.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Serilog
            const string logFolder = "Logs";
            string? logFileName = builder.Configuration.GetValue<string>("Serilog:LogFileName");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.FromLogContext()
                .WriteTo.File(
                path: !string.IsNullOrEmpty(logFileName) ? Path.Combine(builder.Environment.WebRootPath, logFileName) : Path.Combine(builder.Environment.WebRootPath, logFolder, "log-{Date}.txt")
                , rollingInterval: RollingInterval.Day
                , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u11}] {Message:lj} {NewLine}[RequestPath:]{RequestPath} {NewLine}{Exception}"
                )
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);

            // Elasticsearch
            builder.Services.Configure<ElasticsearchSettings>(
                builder.Configuration.GetSection("ElasticsearchSettings"));
            builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();
            
            // Kafka
            builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));
            builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
            
            // AutoMapper
            builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile));

            // DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Repository and UnitOfWork
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IPermissionTypeServices, PermissionTypeServices>();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Apply migrations
            app.MigrateDatabase();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();

                var fileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, logFolder));
                var requestPath = $"{Path.AltDirectorySeparatorChar}{logFolder}";

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = fileProvider,
                    RequestPath = requestPath
                });
                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    FileProvider = fileProvider,
                    RequestPath = requestPath
                });
            //}



            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
