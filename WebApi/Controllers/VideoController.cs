using System.ComponentModel;
using System;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApi.Models;
using FFMpegCore;
using WebApi;
using WebApi.Services;

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

        if (uploaderId == 7)
        {
            sqlQuery = $@"SELECT *
            FROM dbo.videos";
        }


        using (SqlConnection connection = TestDB.GetConnection())
        {
            connection.Open();
            var command = new SqlCommand(sqlQuery, connection);
            if (uploaderId != 7)
            {
                command.Parameters.AddWithValue("@uploaderId", uploaderId);
            }
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
        TimeSpan time = probeResult.Duration;
        int newWidth;
        int newHeight;
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
        if (FFMpeg.Snapshot(videoPath, thumbnailPath, new Size(newWidth, newHeight), time / 2))
        {
            byte[] imageBytes2 = System.IO.File.ReadAllBytes(thumbnailPath);
            return File(imageBytes2, thumbnailType);
        }

        return NotFound("No file was found");
    }

    [HttpPost("Test")]
    [RequestSizeLimit(UploadFolder.MAX_FILE_SIZE)]
    public async Task<IActionResult> Test(IFormFile file, [FromForm] string description, [FromForm] int uploaderId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file");
        }
        if (string.IsNullOrEmpty(description))
        {
            return BadRequest("No description");
        }

        string fileExtension = Path.GetExtension(file.FileName);
        var fileName = "main" + fileExtension;
        var folderPath = Guid.NewGuid().ToString();
        var folderPathFull = Path.Combine(UploadFolder.VIDEOS, folderPath + "\\");
        Directory.CreateDirectory(folderPathFull);

        using (var stream = new FileStream(Path.Combine(folderPathFull, fileName), FileMode.Create))
        {
            file.CopyTo(stream);
        }

        VideoConverter.Main(folderPathFull, fileName);
        Console.WriteLine("testing");

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
                command.Parameters.AddWithValue("@filename", folderPath);
                command.Parameters.AddWithValue("@uploaderid", uploaderId);

                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        }

        return Ok("File uploaded successfully.");
    }

}