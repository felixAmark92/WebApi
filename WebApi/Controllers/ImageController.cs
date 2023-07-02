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
    public IActionResult GetVideo(int Id)
    {
        string? videoName = "";
        TestDB.Connection.Open();
        string sqlQuery =
            $@"SELECT filename
            FROM dbo.videos
            WHERE id = {Id};";

        SqlCommand command = new SqlCommand(sqlQuery, TestDB.Connection);
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            videoName = reader["filename"].ToString();
        }
        reader.Close();
        TestDB.Connection.Close();

        if (videoName == null || videoName == "")
            return BadRequest("video is null");

        string videoPath = Path.Combine(UploadFolder.VIDEOS, videoName);

        if (!System.IO.File.Exists(videoPath))
        {
            return NotFound($"video {videoName} could not be found2");
        }
        Stream filestream = System.IO.File.OpenRead(videoPath);
       

        return File(filestream, "video/mp4", enableRangeProcessing: true);
    }
}
