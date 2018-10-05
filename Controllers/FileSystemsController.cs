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

namespace FileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSystemsController : ControllerBase
    {
        private readonly DataContext _context;
        public FileSystemsController(DataContext context)
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
            var result = new List<FileSystem>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // Đường dẫn tương đối
                    var filePath = Path.Combine("File", DateTime.Now.ToString("yyyyMMdd"));
                    if (!System.IO.File.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);

                    }
                    filePath = Path.Combine(filePath, formFile.FileName);
                    var index = filePath.LastIndexOf(".");
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        // Thêm vào CSDL
                         if (System.IO.File.Exists(filePath))
                        {
                            stream.Close();
                            string hashString = ComputeHashSHA(filePath).Replace("-","");
                        }
                        _context.FileSystem.Add(new FileSystem() { Name = formFile.FileName, Extention = filePath.Substring(index + 1), Size = formFile.Length, UploadedAt = DateTime.Now, Url = filePath });
                        await _context.SaveChangesAsync();
                    }
                    result.Add(new FileSystem() { Name = formFile.FileName, Extention = filePath.Substring(index + 1), Size = formFile.Length, UploadedAt = DateTime.Now, Url = filePath });




                }
            }
            return Ok(result);
        }

        public string ComputeHashSHA(string filename)
        {
            using (var sha = SHA1.Create())
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    return BitConverter.ToString(sha.ComputeHash(stream));
                }
            }
        }
        [HttpPost("DownloadFile")]
        public void Down([FromForm]string address, [FromForm]string fileName)
        {
            string myStringWebResource = null;
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            myStringWebResource = address + "\\" + fileName;
            myWebClient.DownloadFile(myStringWebResource, fileName);
        }
        private bool FileSystemExists(int id)
        {
            return _context.FileSystem.Any(e => e.ID == id);
        }
    }
}