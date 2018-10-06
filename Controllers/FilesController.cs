using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly DataContext _context;
        public FilesController(DataContext context)
        {
            _context = context;
        }

        //// GET: api/FileSystems
        //[HttpGet]
        //public IEnumerable<FileSystem> GetFileSystem()
        //{
        //    return _context.FileSystem;
        //}

        //// GET: api/FileSystems/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetFileSystem([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var fileSystem = await _context.FileSystem.FindAsync(id);

        //    if (fileSystem == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(fileSystem);
        //}

        //// PUT: api/FileSystems/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFileSystem([FromRoute] int id, [FromBody] FileSystem fileSystem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != fileSystem.ID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(fileSystem).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FileSystemExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/FileSystems
        //[HttpPost]
        //public async Task<IActionResult> PostFileSystem([FromBody] FileSystem fileSystem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.FileSystem.Add(fileSystem);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetFileSystem", new { id = fileSystem.ID }, fileSystem);
        //}

        //// DELETE: api/FileSystems/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFileSystem([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var fileSystem = await _context.FileSystem.FindAsync(id);
        //    if (fileSystem == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.FileSystem.Remove(fileSystem);
        //    await _context.SaveChangesAsync();

        //    return Ok(fileSystem);
        //}

        [HttpPost("UploadFiles")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromHeader]List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var result = new List<Files>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // Đường dẫn tương đối
                    var filePath = Path.Combine("Files", DateTime.Now.ToString("yyyyMMdd"));
                    if (!System.IO.File.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);

                    }
                    filePath = Path.Combine(filePath, formFile.FileName);
                    var index = filePath.LastIndexOf(".");
                    string hashString = null;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        
                        // Thêm vào CSDL
                         if (System.IO.File.Exists(filePath))
                        {
                            stream.Close();
                            hashString = ComputeHashSHA(filePath).Replace("-","");
                        }
                        _context.Files.Add(new Files() { Name = formFile.FileName, Hash = hashString, Extention = filePath.Substring(index + 1), Size = formFile.Length, UploadedAt = DateTime.Now, Url = filePath });
                        await _context.SaveChangesAsync();
                    }
                    result.Add(new Files() { Name = formFile.FileName, Hash = hashString, Extention = filePath.Substring(index + 1), Size = formFile.Length, UploadedAt = DateTime.Now, Url = filePath });




                }
            }
            return Ok(result);
        }

        private string ComputeHashSHA(string filename)
        {
            using (var sha = SHA1.Create())
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    return BitConverter.ToString(sha.ComputeHash(stream));
                }
            }
        }


        //public HttpResponseMessage DownloadFile(string fileName)
        //{
        //    if (!string.IsNullOrEmpty(fileName))
        //    {
        //        string filePath = "File";
        //        string fullPath = filePath + "/20181004/" + fileName;
        //        if (System.IO.File.Exists(fullPath))
        //        {

        //            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //            var fileStream = new FileStream(fullPath, FileMode.Open);
        //            response.Content = new StreamContent(fileStream);
        //            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //            response.Content.Headers.ContentDisposition.FileName = fileName;
        //            return response;
        //        }
        //    }
        //    return new HttpResponseMessage(HttpStatusCode.NotFound);
        //}

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = _context.Files.FirstOrDefault(x => x.ID == id);
            string fullPath = file.Url;

            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(fullPath), Path.GetFileName(fullPath));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path);
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", @"application/vnd.openxmlformats
                           officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        private bool FileSystemExists(int id)
        {
            return _context.Files.Any(e => e.ID == id);
        }

        /// <summary>
        /// get file lấy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<FileSystemView> GetById(int id)
        {
            var item = _context.FileSystem.Find(id);
            FileSystemView item2 = new FileSystemView()
            {
                ID = item.ID,
                Name = item.Name,
                Size = item.Size,
                UploadedBy = item.UploadedBy,
                UploadedAt = item.UploadedAt,
                Url = item.Url
            };
            if (item == null)
            {
                return NotFound();
            }


            return item2;

        }
    }
}