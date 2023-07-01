namespace WebApi.Models;

public class UploadedFile
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public int UploaderId { get; set; }
    public string Category { get; set; } = string.Empty;

}
