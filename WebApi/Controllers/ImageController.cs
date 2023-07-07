using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using WebApi.Models;
using FFMpegCore;

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
    [HttpGet("watch")]
    public IActionResult GetVideo(int id)
    {
        VideoModel? video = null;

        string sqlQuery =
            $@"SELECT *
            FROM dbo.videos
            WHERE id = @id";
        using (SqlConnection connection = TestDB.GetConnection())
        {
            connection.Open();
            var command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                video = new VideoModel()
                {
                    Id = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    dateTime = reader.GetDateTime(2).ToShortDateString(),
                    UploaderId = reader.GetInt32(3),
                    FileName = reader.GetString(4)
                };
            }
            reader.Close();
            connection.Close();
        }

        if (video == null)
        {
            return NotFound("requested video not found");
        }
        return Ok(video);
    }

    [HttpGet("thumbnail/{videoName}")]
    public IActionResult GetVideoThumbnail(string videoName)
    {
        string thumbnailPath = UploadFolder.THUMBNAILS + videoName + ".png";
        string thumbnailType = MimeTypes.GetMimeType(thumbnailPath);

        
        if (System.IO.File.Exists(thumbnailPath))
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(thumbnailPath);
            return File(imageBytes, thumbnailType);
        }

        string videoPath = UploadFolder.VIDEOS + videoName;
        var probeResult = FFProbe.Analyse(videoPath);

        int originalWidth = probeResult.PrimaryVideoStream.Width;
        int originalHeight = probeResult.PrimaryVideoStream.Height;
        int newWidth = 0;
        int newHeight = 0;
        if (originalWidth > originalHeight) 
        {
            newWidth = 340;
            newHeight = ((int)(float)newWidth / originalWidth * originalHeight);
        }
        else
        {
            newHeight = 300;
            newWidth = ((int)(float)newHeight / originalHeight * originalWidth);
        }

        if (FFMpeg.Snapshot(videoPath, thumbnailPath, new Size(newWidth, newHeight), TimeSpan.FromSeconds(5)))
        {
            byte[] imageBytes2 = System.IO.File.ReadAllBytes(thumbnailPath);
            return File(imageBytes2, thumbnailType);
        }

        return NotFound("No file was found");
    }

}
