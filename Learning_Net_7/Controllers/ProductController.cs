using Learning_Net_7.Helper;
using Learning_Net_7.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning_Net_7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly LearndataContext _service;
        public ProductController(IWebHostEnvironment environment, LearndataContext service)
        {
            _environment = environment;
            _service = service;
        }
        [NonAction]
        private string GetFilePath(string productCode)
        {
            return _environment.WebRootPath + "\\Upload\\Product\\" + productCode;
        }
        [HttpPut("UploadImage")]
        private async Task<IActionResult> UploadImage(IFormFile file, string productCode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string filePath = GetFilePath(productCode);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                string imagePath = filePath + "\\" + productCode + ".png";

                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using(FileStream stream = System.IO.File.Create(imagePath))
                {
                    await file.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "success";
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }
        
        [HttpPut("MultiUploadImage")]
        private async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0;
            int errorCount = 0;
            try
            {
                
                string filePath = GetFilePath(productCode);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                foreach(var file in fileCollection)
                {
                    string imagePath = filePath + "\\" + file.FileName + ".png";
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        passCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                response.ErrorMessage = ex.Message;
                
            }
            response.ResponseCode = 200;
            response.Result = passCount + " Files uploaded" + (errorCount > 0 ? " & " + errorCount + " files failed" : "");
            return Ok(response);
        }
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string filePath = GetFilePath(productCode);
                string imagePath = filePath + "\\" + productCode + ".png";
                if(System.IO.File.Exists(imagePath))
                {
                    imageUrl = hostUrl + "/Upload/Product/" + productCode + "/" + productCode + ",png";
                }
               
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(imageUrl);
        }
        [HttpGet("GetListImage")]
        public async Task<IActionResult> GetListImage(string productCode)
        {
            List<string> imageUrl = new List<string>();
            string hostUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string filePath = GetFilePath(productCode);
                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string imagePath = filePath + "\\" + fileName;
                        if (System.IO.File.Exists(imagePath))
                        {
                            string _imageUrl = hostUrl + "/Upload/Product/" + productCode + "/" + productCode + ",png";
                            imageUrl.Add(_imageUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(imageUrl);
        }

        [HttpGet("DownLoadImage")]
        public async Task<IActionResult> DownLoadImage(string productCode)
        {
            try
            {
                string filePath = GetFilePath(productCode);
                string imagePath = filePath + "\\" + productCode + ".png";
                if (System.IO.Directory.Exists(filePath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productCode + ".png");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [HttpGet("RemoveImage")]
        public async Task<IActionResult> RemoveImage(string productCode)
        {
            try
            {
                string filePath = GetFilePath(productCode);
                string imagePath = filePath + "\\" + productCode + ".png";
                if (System.IO.Directory.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok("success");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [HttpGet("MultiRemoveImage")]
        public async Task<IActionResult> MultiRemoveImage(string productCode)
        {
            try
            {
                string filePath = GetFilePath(productCode);
                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                    return Ok("success");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection fileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0;
            int errorCount = 0;
            try
            {
                foreach (var file in fileCollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        _service.ProductImages.Add(new Repository.Models.ProductImage()
                        {
                            ProductCode = productCode,
                            ProductImg = stream.ToArray()
                        });
                        await _service.SaveChangesAsync();
                        passCount++;
                    }
                }


            }
            catch (Exception ex)
            {
                errorCount++;
                response.ErrorMessage = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passCount + " Files uploaded" + (errorCount > 0 ? " & " + errorCount + " files failed" : "");
            return Ok(response);
        }


        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string productCode)
        {
            List<string> imageUrl = new List<string>();
            try
            {
                var _productImage = _service.ProductImages.Where(item => item.ProductCode == productCode).ToList();
                if (_productImage != null && _productImage.Count > 0)
                {
                    _productImage.ForEach(item =>
                    {
                        imageUrl.Add(Convert.ToBase64String(item.ProductImg));
                    });
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
            }
            return Ok(imageUrl);

        }


        [HttpGet("DBDowload")]
        public async Task<IActionResult> DBDowload(string productCode)
        {

            try
            {

                var _productimage = await _service.ProductImages.FirstOrDefaultAsync(item => item.ProductCode == productCode);
                if (_productimage != null)
                {
                    return File(_productimage.ProductImg, "image/png", productCode + ".png");
                }

                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }
    }
}
