using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApi.Models;
namespace WebApi.Controllers;


[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{

    // private readonly ApplicationDbContext _context;

    [HttpPost]
    [RequestSizeLimit(UploadFolder.MAX_FILE_SIZE)]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string  description)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file, or no description");

        string fileExtension = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid() + fileExtension;
        var filePath = Path.Combine(UploadFolder.VIDEOS, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        
        string sqlQuery =
            $@"INSERT INTO dbo.videos
            (description, datetime, uploaderid, filename)
            VALUES
            ('{description}', '{DateTime.UtcNow}', 1, '{fileName}')";

        var connection = TestDB.GetConnection();
        connection.Open();
        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        connection.Close();

        return Ok("File uploaded successfully.");
    }

}