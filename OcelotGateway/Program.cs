using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// 🔹 Ocelot config faylı
// ===============================
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ===============================
// 🔹 CORS siyasəti
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ===============================
// 🔹 Swagger Gateway üçün
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "Ocelot API Gateway for Microservices"
    });
});

// ===============================
// 🔹 Ocelot
// ===============================
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// ===============================
// 🔹 Development mode
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseRouting();

// ===============================
// 🔹 Ocelot Middleware
// ===============================
await app.UseOcelot();

app.Run();
