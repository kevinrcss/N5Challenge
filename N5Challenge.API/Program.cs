using Microsoft.EntityFrameworkCore;
using N5Challenge.API.Extensions;
using N5Challenge.Application.Interfaces;
using N5Challenge.Application.Services;
using N5Challenge.Core.Interfaces;
using N5Challenge.Infrastructure.Data;
using N5Challenge.Infrastructure.Kafka;
using N5Challenge.Infrastructure.Repositories;
using N5Challenge.Infrastructure.Services;
using N5Challenge.Infrastructure.Settings;

namespace N5Challenge.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Apply migrations
            app.MigrateDatabase();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var context = services.GetRequiredService<ApplicationDbContext>();
            //        context.Database.Migrate();
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "An error occurred while migrating the database.");
            //    }
            //}

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
