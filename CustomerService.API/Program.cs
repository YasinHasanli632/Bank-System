using CustomerService.Application.Interfaces;
using CustomerService.Application.Services;
using CustomerService.Infrastructure.Data;

using CustomerService.Infrastructure.Repositories;
using CustomerService.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using RedisLibrary.Interfaces;
using RedisLibrary.Implementations;
using StackExchange.Redis;
using SharedLibrary.Messaging.Implementations;
using SharedLibrary.Messaging.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CustomerDb"),
        sql => sql.MigrationsAssembly("CustomerService.Infrastructure")));


builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService.Application.Services.CustomerService>();

  
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

