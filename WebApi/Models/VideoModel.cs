
namespace WebApi.Models;

public class VideoModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime dateTime { get; set; }
    public int UploaderId { get; set; }
    public string FileName { get; set; } = string.Empty;

}
