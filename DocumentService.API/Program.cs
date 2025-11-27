using DocumentService.Application.Interfaces;
using DocumentService.Application.Services;
using DocumentService.Infrastructure.Data;

using DocumentService.Infrastructure.Repositories.Implementations;
using DocumentService.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using RedisLibrary.Implementations;
using RedisLibrary.Interfaces;
using SharedLibrary.Messaging.Implementations;
using SharedLibrary.Messaging.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DocumentDb"),
        sql => sql.MigrationsAssembly("DocumentService.Infrastructure")));


builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService.Application.Services.DocumentService>();



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

// ===============================
// 🔹 Dummy Cache (Redis işləməzsə)
// ===============================

