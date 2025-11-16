
using Microsoft.AspNetCore.Mvc;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Qoutes.Application.Dtos;
using MiniPricingApp.Modules.Qoutes.Application.Services;
using System.Threading.Channels;

namespace MiniPricingApp.Modules.qoutes
{
    /// <summary>
    /// Manages creation, Update, and retrieval of pricing rules.
    /// </summary>
    [ApiController]
    [Route("")]
    public class QouteController : Controller
    {
        private readonly ILogger<QouteController> _logger;
        private readonly IQouteService _qouteService;


         public QouteController(ILogger<QouteController> logger, IQouteService qoutesService)
        {
            _logger = logger;
            _qouteService = qoutesService;
        }

        [HttpPost("/qoutes/price")]
        [EndpointSummary("คำนวณราคาค่าจัดส่งจากน้ำหนักและรหัสพื้นที่.")]
        [EndpointDescription("เอ็นพอยต์นี้ใช้สำหรับคำนวณราคาค่าจัดส่งจากน้ำหนักและรหัสพื้นที่ที่ส่งเข้ามา ระบบจะตรวจสอบความถูกต้องของน้ำหนักโดยเทียบกับกฎ WeightTier ที่กำหนดไว้ จากนั้นจะโหลดกฎการตั้งราคาที่เปิดใช้งานทั้งหมด เช่น ส่วนลด ค่าธรรมเนียมพื้นที่ห่างไกล หรือโปรโมชัน และประมวลผลตามลำดับความสำคัญของกฎแต่ละชุด ผลลัพธ์ที่ได้จะเป็นราคาค่าจัดส่งสุดท้ายหลังจากประยุกต์กฎทั้งหมดแล้ว.")]
        public IActionResult CalculatePrice([FromBody] PricingCalculationRequestDto request)
        {
            _logger.LogInformation($"RECIEVE CALCULATE PRICE REQUEST ==> {request}");
            return Ok(_qouteService.CalculatePrice(request));
        }

        [HttpPost("/qoutes/bulk")]
        [Consumes("multipart/form-data")]
        [EndpointSummary("อัปโหลดไฟล์ CSV เพื่อสร้างข้อมูลใบเสนอราคาจำนวนมากแบบ Bulk.")]
        [EndpointDescription("อัปโหลดไฟล์ CSV สำหรับนำเข้าข้อมูลจำนวนมาก โดยเอ็นพอยต์นี้รองรับเฉพาะไฟล์ CSV เท่านั้น เมื่ออัปโหลดสำเร็จระบบจะส่งกลับ Job Id ซึ่งคุณสามารถใช้เพื่อตรวจสอบสถานะของงานได้ หลังจากอัปโหลดแล้วควรรอประมาณ 1 นาทีก่อนทำการตรวจสอบสถานะอีกครั้ง เนื่องจากระบบมี Background Task ที่ทำงานทุก ๆ 1 นาทีเพื่อตรวจสอบและประมวลผลไฟล์ที่อัปโหลด การรอจึงเป็นสิ่งจำเป็นเพื่อให้ Background Task มีเวลาเริ่มประมวลผลไฟล์ดังกล่าวอย่างถูกต้อง")]

        public async Task<IActionResult> BulkFile([FromForm] UploadCsvFileRequestDto request)
        {

            _logger.LogInformation($"RECIEVE BULK REQUEST ==> {request.file}");
            return Ok(await _qouteService.SaveFromFile(request.file));
        }

        [HttpGet("/jobs/all")]
        [EndpointSummary("ดึงข้อมูลสถานะงานทังหมด")]
        public async Task<IActionResult> GetAllJob()
        {

            _logger.LogInformation($"RECIEVE QOERY PARAM TO GET ALL JOB ==>");
            return Ok(await _qouteService.GetManyJob());
        }

        [HttpGet("/jobs/{Id}")]
        [EndpointSummary("ดึงข้อมูลสถานะงาน (Job) ตาม Job Id.")]
        [EndpointDescription("เอ็นพอยต์นี้ใช้สำหรับตรวจสอบสถานะของงาน (Job) โดยใช้ Job Id ที่ได้รับหลังจากอัปโหลดไฟล์ CSV หรือสร้างงานอื่น ๆ ระบบจะส่งกลับข้อมูลสถานะปัจจุบันของงาน เช่น PENDING, COMPLETED หรือ FAILED เพื่อตรวจสอบว่าการประมวลผลเสร็จสมบูรณ์หรือไม่.")]
        public async Task<IActionResult> GetJobById(Guid Id)
        {

            _logger.LogInformation($"RECIEVE QOERY PARAM TO GET JOB ==> {Id}");
            return Ok(await _qouteService.GetJobById(Id));
        }

    }
}
