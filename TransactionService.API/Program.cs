using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Services;
using TransactionService.Infrastructure.Data;

using TransactionService.Infrastructure.Repositories.Implementations;
using TransactionService.Infrastructure.Repositories.Interfaces;
using RedisLibrary.Interfaces;
using RedisLibrary.Implementations;
using SharedLibrary.Messaging.Implementations;
using SharedLibrary.Messaging.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TransactionDb"),
        sql => sql.MigrationsAssembly("TransactionService.Infrastructure")));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService.Application.Services.TransactionService>();



    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect("localhost:6379"));

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
        options.InstanceName = "BankSystem_";
    });

    builder.Services.AddScoped<ICacheService, RedisCacheService>();


builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});


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


