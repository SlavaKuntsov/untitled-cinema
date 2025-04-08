using Microsoft.AspNetCore.Mvc;
using Minios.Services;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("api/storage")]
public class StorageController(IMinioService minioService) : ControllerBase
{
	[HttpPost("upload")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> Upload(IFormFile? file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("No file uploaded");

		var objectName = Guid.NewGuid() + Path.GetExtension(file.FileName);

		await using var stream = file.OpenReadStream();
		await minioService.UploadFileAsync(null, objectName, stream, file.ContentType);

		return Ok(new { Message = "File uploaded", FileName = objectName });
	}

	[HttpGet("download/{fileName}")]
	public async Task<IActionResult> Download(string fileName)
	{
		var stream = await minioService.GetFileAsync(null, fileName);

		return File(stream, "application/octet-stream", fileName);
	}

	[HttpDelete("{fileName}")]
	public async Task<IActionResult> Delete(string fileName)
	{
		await minioService.RemoveFileAsync(null, fileName);

		return Ok(new { Message = $"File {fileName} deleted" });
	}

	[HttpGet("url/{fileName}")]
	public async Task<IActionResult> GetPresignedUrl(string fileName, [FromQuery] int expiry = 3600)
	{
		var url = await minioService.GetPresignedUrlAsync(null, fileName, expiry);

		return Ok(new { Url = url });
	}

	[HttpGet("url/files")]
	public async Task<IActionResult> ListUrlFiles([FromQuery] int expiry = 3600)
	{
		var files = await minioService.ListFilesAsync();
		var urlFiles = default(IList<string>);

		foreach (var file in files)
		{
			var url = await minioService.GetPresignedUrlAsync(null, file.Name, expiry);

			urlFiles.Add(url);
		}

		return Ok(urlFiles);
	}

	[HttpGet("files")]
	public async Task<IActionResult> ListFiles()
	{
		var files = await minioService.ListFilesAsync();

		return Ok(files);
	}

	[HttpGet("files/{fileName}/meta")]
	public async Task<IActionResult> GetFileMetadata(string fileName)
	{
		var metadata = await minioService.GetFileMetadataAsync(null, fileName);

		return Ok(metadata);
	}
}