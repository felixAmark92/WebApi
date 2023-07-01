using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
namespace WebApi.Controllers;


[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{

    // private readonly ApplicationDbContext _context;

    [HttpPost]
    [RequestSizeLimit(UploadFolder.MAX_FILE_SIZE)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsFolderPath = Path.Combine(UploadFolder.IMAGES);
        if (!Directory.Exists(uploadsFolderPath))
            Directory.CreateDirectory(uploadsFolderPath);

        string fileExtension = Path.GetExtension(file.FileName);

        var filePath = Path.Combine(uploadsFolderPath, Guid.NewGuid() + fileExtension);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var newFile = new UploadedFile
        {
            //Description = description,
            UploadDate = DateTime.Now,
            UploaderId = 1,
            Category = "none",
            //FilePath = filePath,
        };

        //_context.UploadedFiles.Add(newFile);
        //await _context.SaveChangesAsync();

        return Ok("File uploaded successfully.");
    }

}