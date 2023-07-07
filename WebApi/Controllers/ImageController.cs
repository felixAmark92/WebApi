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
    [HttpGet("{videoName}")]
    public IActionResult GetVideo(string videoName)
    {
        string videoPath = Path.Combine(UploadFolder.VIDEOS, videoName);

        if (!System.IO.File.Exists(videoPath))
        {
            var notFoundMessage = new string[] { "Video could not be found", $"Path: {videoPath}" };
            return NotFound(notFoundMessage);
        }
        Stream filestream = System.IO.File.OpenRead(videoPath);

        string fileType = MimeTypes.GetMimeType(videoPath);

        if (fileType == "application/mp4")
        {
            fileType = "video/mp4";
        }

        return File(filestream, fileType, enableRangeProcessing: true);
    }
    [HttpPost]
    public IActionResult GetVideos(int uploaderId)
    {
        var videos = new List<VideoModel>();

        string sqlQuery =
            $@"SELECT *
            FROM dbo.videos
            WHERE uploaderid = @uploaderId";
        using (SqlConnection connection = TestDB.GetConnection())
        {
            connection.Open();
            var command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@uploaderId", uploaderId);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                // DateTime dateTime = reader.GetDateTime(2);
                // string test = dateTime.ToShortDateString();
                // string test2 = test;

                var video = new VideoModel()
                {
                    Id = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    dateTime = reader.GetDateTime(2).ToShortDateString(),
                    UploaderId = reader.GetInt32(3),
                    FileName = reader.GetString(4)
                };
                videos.Add(video);
            }
            reader.Close();
            connection.Close();
        }

        if (videos == null || videos.Count == 0)
        {
            return NotFound("no videos found");
        }
        return Ok(videos);
    }
}
