
using FileStorageService.Application;
using FileStorageService.Application.Services;
using FileStorageService.Infrastructure.Data;

using FileStorageService.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using RedisLibrary.Interfaces;
using RedisLibrary.Implementations;
using StackExchange.Redis;
using SharedLibrary.Messaging.Implementations;
using SharedLibrary.Messaging.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<FileStorageDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FileStorageDb"),
        sql => sql.MigrationsAssembly("FileStorageService.Infrastructure")));


builder.Services.AddScoped<IFileStorageRepository, FileStorageRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService.Application.Services.FileStorageService>();



    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect("localhost:6379"));

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
        options.InstanceName = "BankSystem_";
    });

   
    builder.Services.AddScoped<ICacheService, RedisCacheService>();




builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();




