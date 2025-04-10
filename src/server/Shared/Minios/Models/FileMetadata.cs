namespace Minios.Models;

public class FileMetadata
{
	public string Name { get; set; } = null!;
	public long Size { get; set; }
	public DateTime LastModified { get; set; }
}