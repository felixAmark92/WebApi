using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;



[ApiController]
[Route("[controller]")]
public class DateController : ControllerBase
{

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(DateTime.UtcNow);
    }
}

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{imageName}")]
    public IActionResult GetImage(string imageName)
    {
        var imagePath = Path.Combine("\\\\192.168.0.200\\felix-share\\UbisoftConnect\\ok\\ref", imageName);

        if (!System.IO.File.Exists(imagePath))
        {
            return NotFound();
        }
        var imageBytes = System.IO.File.ReadAllBytes(imagePath);
        var imageContentType = MimeTypes.GetMimeType(imageName); 

        return File(imageBytes, imageContentType);
    }
}