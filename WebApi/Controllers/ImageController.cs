using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{imageName}")]
    public async Task<IActionResult> GetImage(string imageName)
    {
        string imagePath = Path.Combine(UploadFolder.IMAGES_PATH, imageName);

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
        string videoPath = Path.Combine(UploadFolder.IMAGES_PATH, videoName);

        if (!System.IO.File.Exists(videoPath))
        {
            return NotFound("no file");
        }
        var filestream = System.IO.File.OpenRead(videoPath);
       
        string videoContentType = MimeTypes.GetMimeType(videoName);

        return File(filestream, videoContentType, enableRangeProcessing: true);
    }
}
