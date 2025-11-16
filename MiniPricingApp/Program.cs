using Microsoft.Extensions.Hosting;
using MiniPricingApp.Middleware;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Infrastructure.BackgrounTask;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.Implemtation;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces;
using MiniPricingApp.Modules.Qoutes.Application.Services;
using MiniPricingApp.Modules.Qoutes.Domain.Interfaces;
using MiniPricingApp.Modules.Qoutes.Domain.Validators;
using MiniPricingApp.Modules.Rules.Application.Services;
using MiniPricingApp.Modules.Rules.Domains.Factory;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Domains.Services;
using MiniPricingApp.Modules.Rules.Domains.Validator;
using MiniPricingApp.Modules.Rules.Infrastructure.Implementations;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;
using Serilog;
using System.Reflection;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
}); ;

builder.Services.AddOpenApi("v1");
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()              // enrich with context properties
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/pricing-mini-app-.log", rollingInterval: RollingInterval.Day)
    //.WriteTo.Seq("http://localhost:5341") // optional Seq
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// dependency injection
builder.Services.AddOpenApi();


builder.Services.AddScoped<IQouteRepository, QouteRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();
builder.Services.AddScoped<IQouteService, QouteService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddHostedService<BulkQouteJobWorker>();

builder.Services.AddScoped<IWeightTierDomainService, WeightTierDomainService>();
builder.Services.AddScoped<PricingRuleFactory>();
builder.Services.AddScoped<WeightTierValidator>();
builder.Services.AddScoped<IPricingRuleValidatorFactory, PricingRuleValidatorFactory>();
builder.Services.AddScoped<IQouteWeightTierValidator, QouteWeightValidator>();
builder.Services.AddScoped<IQouteCsvValidator, QouteCsvValidator>();



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint("/openapi/v1.json", "Pricing Mini App Document ");
});

app.UseReDoc(option =>
{
    option.SpecUrl("/openapi/v1.json");
});

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();

app.Run();

// Add this at the very end of Program.cs
public partial class Program { }  // empty class, used only for integration tests
