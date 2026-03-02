using Incremental.Core.Managers;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.ModelFactories.Interfaces;
using Incremental.Core.Services;
using Incremental.Core.Services.Interfaces;
using Incremental.Data;
using Incremental.Data.Interfaces;
using IncrementGame.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace IncrementGame.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build())
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                   "{Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] " +
                                   "{Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 7)
                .CreateLogger();

            try
            {
                Log.Information("Запуск приложения...");

                var builder = WebApplication.CreateBuilder(args);

                // Добавляем Serilog в контейнер
                builder.Host.UseSerilog();

                // Add services to the container.

                builder.Services.AddControllers();

                builder.Services.AddSignalR();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.SetIsOriginAllowed(origin => true) // Для SignalR важно!
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials(); // SignalR требует AllowCredentials
                    });
                });

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddDbContext<ProjectContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddScoped<IPointManager, SingleGamePointManager>();
                builder.Services.AddScoped<IUpgradeManager, UpgradeManager>();
                builder.Services.AddScoped<IGameCalculationService, GameCalculationService>();
                builder.Services.AddScoped<IDataInitializer, DataInitializer>();
                builder.Services.AddScoped<SingleGameCacheManager>();

                var app = builder.Build();

                app.UseDefaultFiles();
                app.UseStaticFiles();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();

                    try
                    {
                        using (var scope = app.Services.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ProjectContext>();

                            // Проверяем, может ли подключиться к БД
                            if (dbContext.Database.CanConnect())
                            {
                                dbContext.Database.Migrate();
                                Console.WriteLine("Миграции успешно применены");
                            }
                            else
                            {
                                Console.WriteLine("Нет подключения к БД, создаю БД...");
                                dbContext.Database.EnsureCreated(); // Создаёт БД, если её нет
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Ошибка при применении миграций");
                    }
                }

                using (var scope = app.Services.CreateScope())
                {
                    var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
                    await initializer.InitializeAsync();  
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowAll");

                app.UseAuthorization();


                app.MapControllers();

                app.MapHub<GameHub>("/gameHub");

                app.MapFallbackToFile("/index.html");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Приложение завершилось с критической ошибкой");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
