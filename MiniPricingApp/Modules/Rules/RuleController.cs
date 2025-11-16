using Microsoft.AspNetCore.Mvc;
using MiniPricingApp.Modules.qoutes;
using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Application.Services;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Shares.Common;
using Serilog.Sinks.File;
using System.Threading.Tasks;

namespace MiniPricingApp.Modules.Rules
{
    /// <summary>
    /// Controller for managing pricing rules.
    /// Provides endpoints to create, Update, and retrieve pricing rules.
    /// </summary>
    [ApiController]
    [Route("rules")]
    public class RuleController : Controller
    {
        private readonly ILogger<QouteController> _logger;
        private readonly IRuleService _ruleService;
        public RuleController(IRuleService ruleService, ILogger<QouteController> logger)
        {
            _ruleService = ruleService;
            _logger = logger;
        }

        [HttpPost()]
        [EndpointSummary("สร้างกฎราคาค่าขนส่งใหม่ (Pricing Rule)")]
        [EndpointDescription("เอ็นพอยต์นี้ใช้สำหรับสร้างกฎราคาค่าขนส่งใหม่ โดยรองรับ 3 ประเภทหลักของกฎราคา: \n\n" +
        "1. WeightTier: ผู้ใช้ต้องระบุเพียง MaxWeight เท่านั้น ระบบจะคำนวณ MinWeight ให้อัตโนมัติจาก WeightTier ก่อนหน้าเพื่อป้องกันช่องว่างหรือความทับซ้อนระหว่างช่วงน้ำหนัก \n" +
        "2. TimeWindowPromotion: ผู้ใช้ต้องระบุ EffectiveFrom, EffectiveTo และ distcountPercent เท่านั้น เพื่อกำหนดช่วงเวลาและส่วนลดของโปรโมชั่น \n" +
        "3. RemoteAreaSurcharge: ผู้ใช้ต้องระบุเพียง AreaCode และ Surcharge เพื่อกำหนดค่าธรรมเนียมเพิ่มเติมสำหรับพื้นที่ห่างไกล \n\n" +
        "ระบบจะตรวจสอบข้อมูลที่ส่งเข้ามาและสร้างกฎราคาตามประเภทที่เลือก พร้อมคืนค่า Id ของกฎราคาที่สร้างสำเร็จ")]
        public async Task<IActionResult> CreateRule([FromBody] PricingRuleRequestDto request)
        {
            _logger.LogInformation($"RECIEVE REQUEST TO CREATE RULE ==> {request}");
            var result = await _ruleService.CreateRule(request);
            return Ok(result);
        }

        [HttpPut("/{Id}")]
        [EndpointSummary("อัปเดตกฎราคาค่าขนส่งที่มีอยู่")]
        [EndpointDescription("เอ็นพอยต์นี้ใช้สำหรับอัปเดตกฎราคาค่าขนส่งที่มีอยู่แล้ว โดยต้องระบุ RuleType ทุกครั้งเพื่อให้ระบบทราบประเภทของกฎราคา เช่น WeightTier, TimeWindowPromotion หรือ RemoteAreaSurcharge \n\n" +
        "เหตุผลที่ต้องระบุ RuleType ทุกครั้ง: \n" +
        "- การอัปเดตอาจเปลี่ยนค่าของกฎราคาหลายฟิลด์ที่เกี่ยวข้องกับประเภทนั้น ๆ \n" +
        "- WeightTier: ผู้ใช้ยังสามารถระบุ MaxWeight เพื่อปรับช่วงน้ำหนัก ระบบจะคำนวณ MinWeight ให้อัตโนมัติ \n" +
        "- TimeWindowPromotion: ต้องระบุ EffectiveFrom, EffectiveTo และ distcountPercent เพื่อปรับช่วงเวลาและส่วนลด \n" +
        "- RemoteAreaSurcharge: ต้องระบุ AreaCode และ Surcharge เพื่อปรับค่าธรรมเนียมพื้นที่ห่างไกล \n\n" +
        "ระบบจะตรวจสอบข้อมูลที่ส่งเข้ามาและอัปเดตกฎราคาให้ตรงกับประเภทที่ระบุ พร้อมคืนค่า Id ของกฎราคาที่อัปเดตสำเร็จ")]
        public async Task<IActionResult> UpdateRule(Guid Id, [FromBody] PricingRuleRequestDto request)
        {
            _logger.LogInformation($"RECIEVE REQUEST TO Update RULE ==> {Id}");
            var result = await _ruleService.UpdateRule(Id, request);
            return Ok(result);
        }

        [HttpDelete("/{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            _logger.LogInformation($"RECIEVE QOERY TO GET RULE BY ID ==> {Id}");
            var result = await _ruleService.DeleteRule(Id);
            return Ok(result);
        }


        [HttpGet("/{Id}")]
        [EndpointSummary("ดึงข้อมูลฎราคา")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            _logger.LogInformation($"RECIEVE QOURY PARAM TO GET RULES ==> ");
            var rules = _ruleService.GetRuleById(Id);
            return Ok(rules);
        }

        [HttpGet("all")]
        [EndpointSummary("ดึงข้อมูลฎราคาทังหมด")]
        public async Task<IActionResult> GetRules()
        {
            _logger.LogInformation($"RECIEVE QOURY PARAM TO GET RULES ==> ");
            var rules = _ruleService.GetRules();
            return Ok(rules);
        }
    }
}
