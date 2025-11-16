using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Application.Mapper;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Factory;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;
using System.Text.Json;

namespace MiniPricingApp.Modules.Rules.Infrastructure.Implementations
{
    public class PricingRuleRepository : IPricingRuleRepository
    {
      private String FILE_NAME = "pricing_rules.json";
      private readonly   IWebHostEnvironment _env;

       public  PricingRuleRepository(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<bool> Detele(Guid Id)
        {
            var path = Path.Combine(
          _env.ContentRootPath,
          "Modules",
          "Rules",
          "Infrastructure",
           FILE_NAME
      );

            // Ensure file exists
            if (!File.Exists(path))
            {
                await File.WriteAllTextAsync(path, "[]"); // create empty JSON array
            }

            var json = File.ReadAllText(path);
            var rulesDto = JsonSerializer.Deserialize<List<PricingRuleDto>>(json);
            var rules = rulesDto.Select(t => BasePricingRuleMapper.MapToDomain(t)).ToList();

            rules = rules.Where(t => t.Id != Id).ToList();

            var UpdatedJson = JsonSerializer.Serialize(
                rules,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return true; // or whatever ID you generate
        }

        public BasePricingRule GetById(Guid id)
        {
            var path = Path.Combine(
                 _env.ContentRootPath,
                 "Modules",
                 "Rules",
                 "Infrastructure",
                 FILE_NAME
             );
            var json = File.ReadAllText(path);
            var ruleDtos = JsonSerializer.Deserialize<List<PricingRuleDto>>(json);

            var rules = ruleDtos.Select(t => BasePricingRuleMapper.MapToDomain(t)).ToList();
            return rules.FirstOrDefault(t => t.Id  == id);
        }

        public WeightTierEntity findLastMaxWeightTier()
        {
            var path = Path.Combine(
                  _env.ContentRootPath,
                  "Modules",
                  "Rules",
                  "Infrastructure",
                  FILE_NAME
              );
            var json = File.ReadAllText(path);
            var rulesDto = JsonSerializer.Deserialize<List<PricingRuleDto>>(json);
            var rules = rulesDto.Select(t => BasePricingRuleMapper.MapToDomain(t)).ToList();
            if (rules.Count() <= 0)
            {
                return null;
            }
            var weightTier = rules
                .OfType<WeightTierEntity>()
                .OrderByDescending(w => w.MaxWeight)
                .First();

            return weightTier;
        }

        public List<BasePricingRule> GetMany()
        {
            try
            {

                var path = Path.Combine(
                    _env.ContentRootPath,
                    "Modules",
                    "Rules",
                    "Infrastructure",
                    FILE_NAME
                );
                var json = File.ReadAllText(path);
                var rulesDto = JsonSerializer.Deserialize<List<PricingRuleDto>>(json);
                var rules = rulesDto.Select(t => BasePricingRuleMapper.MapToDomain(t)).ToList();


                return rules;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error occurred", ex);
            }
        }

        public async Task<Guid> Save(BasePricingRule entity)
        {
            var path = Path.Combine(
                _env.ContentRootPath,
                "Modules",
                "Rules",
                "Infrastructure",
                 FILE_NAME
            );
    
            // Ensure file exists
            if (!File.Exists(path))
            {
                await File.WriteAllTextAsync(path, "[]"); // create empty JSON array
            }

            var json = File.ReadAllText(path);
            var rulesDto = JsonSerializer.Deserialize<List<PricingRuleDto>>(json);
            var rules = rulesDto.Select(t => BasePricingRuleMapper.MapToDomain(t)).ToList();

            rules.Add(entity);

            var UpdatedJson = JsonSerializer.Serialize(
                rules,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return entity.Id.Value; // or whatever ID you generate
        }

        public Task<bool> SaveMany(List<BasePricingRule> entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> Update(BasePricingRule entity)
        {
            var path = Path.Combine(
              _env.ContentRootPath,
              "Modules",
              "Rules",
              "Infrastructure",
              FILE_NAME
          );
            // Ensure file exists
            if (!File.Exists(path))
            {
                await File.WriteAllTextAsync(path, "[]"); // create empty JSON array
            }

            var json = File.ReadAllText(path);
            var ruleDtos = JsonSerializer.Deserialize<List<PricingRuleDto>>(json);
            var rules = ruleDtos.Select(t => BasePricingRuleMapper.MapToDomain(t))
                             .OrderBy(r => r.Priority)
                             .ToList();

            BasePricingRule existing = rules.FirstOrDefault(t => t.Id == entity.Id);

            if (existing == null)
                throw new Exception($"Rules with Id {entity.Id} not found.");


            rules.Remove(existing);
            rules.Add(entity);
            

            var UpdatedJson = JsonSerializer.Serialize(
                rules,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return entity.Id.Value; // or whatever ID you generate
        }

        public Task<bool> UpdateMany(List<BasePricingRule> entity)
        {
            throw new NotImplementedException();
        }
    }
}
