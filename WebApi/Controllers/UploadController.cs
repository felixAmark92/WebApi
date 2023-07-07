using System.Data.Common;
using System.ComponentModel.Design;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApi.Models;
namespace WebApi.Controllers;


[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(UploadFolder.MAX_FILE_SIZE)]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string description, [FromForm] int uploaderId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file, or no description");
        }


        string fileExtension = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid() + fileExtension;
        var filePath = Path.Combine(UploadFolder.VIDEOS, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        string sqlQuery =
            @"INSERT INTO dbo.videos
            (description, datetime, uploaderid, filename)
            VALUES
            (@description, @datetime, @uploaderid, @filename)";

        using (SqlConnection connection = TestDB.GetConnection())
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@datetime", DateTime.UtcNow);
                command.Parameters.AddWithValue("@filename", fileName);
                command.Parameters.AddWithValue("@uploaderid", uploaderId);

                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        }

        return Ok("File uploaded successfully.");
    }

}