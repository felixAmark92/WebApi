
namespace WebApi.Models;

public class VideoModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string dateTime { get; set; } = string.Empty;
    public int UploaderId { get; set; }
    public string FileName { get; set; } = string.Empty;

}
