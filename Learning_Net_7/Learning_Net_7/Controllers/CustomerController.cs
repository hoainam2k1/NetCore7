using ClosedXML.Excel;
using Learning_Net_7.Modal;
using Learning_Net_7.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;

namespace Learning_Net_7.Controllers
{
    //[DisableCors]
    //[Authorize]
    [EnableRateLimiting("fixedwindow")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly IWebHostEnvironment _environment;
        public CustomerController(ICustomerService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment; 
        }
        [NonAction]
        private string GetFilePath()
        {
            return _environment.WebRootPath + "\\ExportExcel";
        }
        //[EnableCors("corsPolicy1")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var listCustomer = await _service.GetAll();
            if (listCustomer.Count() > 0)
            {
                return Ok(listCustomer);
            }
            return NotFound();
        }
        [DisableRateLimiting]
        [HttpGet("GetByCode")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var customer = await _service.GetByCode(code);
            if (customer != null)
            {
                return Ok(customer);
            }
            return NotFound();
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CustomerModal modal)
        {
            var customer = await _service.Create(modal);
            return Ok(customer);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Create(CustomerModal modal, string code)
        {
            var customer = await _service.Update(modal, code);
            return Ok(customer);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string code)
        {
            var customer = await _service.Remove(code);
            return Ok(customer);
        }
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string filePath = GetFilePath();
                string excelPath = filePath + "\\customerinfo.xlsx";
                DataTable dt = new DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("Creditlimit", typeof(string));

                var listCustomer = await _service.GetAll();
                if (listCustomer.Count() > 0)
                {
                    listCustomer.ForEach(item =>
                    {
                        dt.Rows.Add(item.Code, item.Name, item.Email, item.Phone, item.Creditlimit);
                    });
                }
                using (XLWorkbook workBook = new XLWorkbook())
                {
                    workBook.AddWorksheet(dt, "Customer Info");
                    using(MemoryStream stream = new MemoryStream())
                    {
                        workBook.SaveAs(stream);

                        if (System.IO.File.Exists(excelPath))
                        {
                            System.IO.File.Delete(excelPath);
                        }
                        workBook.SaveAs(excelPath);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
