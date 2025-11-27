using AccountService.Application.Interfaces;
using AccountService.Application.Services;
using AccountService.Infrastructure.Data;

using AccountService.Infrastructure.Repositories;
using AccountService.Infrastructure.Repositories.Implementations;
using AccountService.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using RedisLibrary.Implementations;
using RedisLibrary.Interfaces;
using SharedLibrary.Messaging.Implementations;
using SharedLibrary.Messaging.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AccountDb"),
        sql => sql.MigrationsAssembly("AccountService.Infrastructure")));


builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService.Application.Services.AccountService>();


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

