using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProjectDataAccess.DbModel;
using RiskAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IDataContainerRiskService,DataContainerRiskService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Risk API" }));
builder.Services.AddDbContext<Risk_DbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("PA_RISK")));
var app = builder.Build();

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
