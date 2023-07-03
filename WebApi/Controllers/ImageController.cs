using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using WebApi.Models;


namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{imageName}")]
    public async Task<IActionResult> GetImage(string imageName)
    {
        string imagePath = Path.Combine(UploadFolder.IMAGES, imageName);

        if (!System.IO.File.Exists(imagePath))
        {
            return NotFound();
        }
        byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
        string imageContentType = MimeTypes.GetMimeType(imageName);

        return File(imageBytes, imageContentType);
    }
}

[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetVideo(int Id)
    {
        string? videoName = "";

        string sqlQuery =
            $@"SELECT filename
            FROM dbo.videos
            WHERE id = {Id}";
        using (SqlConnection connection = TestDB.GetConnection())
        {
            await connection.OpenAsync();
            var command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (await reader.ReadAsync())
            {
                videoName = reader["filename"].ToString();
            }
            reader.Close();
            await connection.CloseAsync();
        }

        if (videoName == null || videoName == "")
            return BadRequest("video is null");

        string videoPath = Path.Combine(UploadFolder.VIDEOS, videoName);

        if (!System.IO.File.Exists(videoPath))
        {
            return NotFound($"video {videoName} could not be found2");
        }
        Stream filestream = System.IO.File.OpenRead(videoPath);

        string fileType = MimeTypes.GetMimeType(videoPath);
        if (fileType == "application/mp4")
        {
            fileType = "video/mp4";
        }

        return File(filestream, fileType, enableRangeProcessing: true);
    }
}
