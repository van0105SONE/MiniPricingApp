using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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


namespace MiniPricingApp.Tests.Integrate.Factory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public CustomWebApplicationFactory()
        {
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.AddScoped<IQouteRepository, QouteRepository>();
                services.AddScoped<IJobRepository, JobRepository>();
                services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();
                services.AddScoped<IQouteService, QouteService>();
                services.AddScoped<IRuleService, RuleService>();
                services.AddHostedService<BulkQouteJobWorker>();

                services.AddScoped<IWeightTierDomainService, WeightTierDomainService>();
                services.AddScoped<PricingRuleFactory>();
                services.AddScoped<WeightTierValidator>();
                services.AddScoped<IPricingRuleValidatorFactory, PricingRuleValidatorFactory>();
                services.AddScoped<IQouteWeightTierValidator, QouteWeightValidator>();
                services.AddScoped<IQouteCsvValidator, QouteCsvValidator>();
            });
        }
    }
}
