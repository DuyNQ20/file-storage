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

        // GET: api/FileSystems
        [HttpGet]
        public IEnumerable<FileSystem> GetFileStorage()
        {
            return _context.FileStorage;
        }

        // GET: api/FileSystems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileSystem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileSystem = await _context.FileStorage.FindAsync(id);

            if (fileSystem == null)
            {
                return NotFound();
            }

            return Ok(fileSystem);
        }

        // PUT: api/FileSystems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileSystem([FromServices] int id, [FromBody] FileSystem fileSystem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fileSystem.ID)
            {
                return BadRequest();
            }

            _context.Entry(fileSystem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileSystemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FileSystems
        //[HttpPost]
        //public async Task<IActionResult> PostFileSystem([FromBody] FileSystem fileSystem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.FileStorage.Add(fileSystem);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetFileSystem", new { id = fileSystem.ID }, fileSystem);
        //}
        
        [HttpPost("UploadFiles")]
       
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm]List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }

        // DELETE: api/FileSystems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileSystem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileSystem = await _context.FileStorage.FindAsync(id);
            if (fileSystem == null)
            {
                return NotFound();
            }

            _context.FileStorage.Remove(fileSystem);
            await _context.SaveChangesAsync();

            return Ok(fileSystem);
        }

        private bool FileSystemExists(int id)
        {
            return _context.FileStorage.Any(e => e.ID == id);
        }
    }
}