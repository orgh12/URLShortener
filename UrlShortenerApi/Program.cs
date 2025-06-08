using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => {options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); });
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUrlService, UrlService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();