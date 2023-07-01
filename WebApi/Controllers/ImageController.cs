using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{imageName}")]
    public IActionResult GetImage(string imageName)
    {
        string imagePath = Path.Combine(UploadFolder.IMAGES, imageName);

        if (!System.IO.File.Exists(imagePath))
        {
            return NotFound();
        }
        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        string imageContentType = MimeTypes.GetMimeType(imageName); 

        return File(imageBytes, imageContentType);
    }
}
