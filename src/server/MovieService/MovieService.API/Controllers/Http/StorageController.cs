using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;
using Minios;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("api/storage")]
public class StorageController(
	IMinioClient minioClient,
	IOptions<MinioOptions> options,
	ILogger<StorageController> logger)
	: ControllerBase
{
	private readonly MinioOptions _options = options.Value;

	// Загрузка файла
	[HttpPost("upload")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> UploadFile(IFormFile? file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("No file uploaded");

		var objectName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

		await using var stream = file.OpenReadStream();

		var putObjectArgs = new PutObjectArgs()
			.WithBucket(_options.DefaultBucket)
			.WithObject(objectName)
			.WithStreamData(stream)
			.WithObjectSize(stream.Length)
			.WithContentType(file.ContentType);

		await minioClient.PutObjectAsync(putObjectArgs);

		logger.LogInformation($"File {objectName} uploaded successfully");

		return Ok(
			new
			{
				FileName = objectName,
				Size = file.Length,
				file.ContentType
			});
	}

	// Получение списка всех файлов
	[HttpGet("files")]
	public async Task<IActionResult> GetAllFiles()
	{
		var listArgs = new ListObjectsArgs()
			.WithBucket(_options.DefaultBucket)
			.WithRecursive(true);

		var files = new List<object>();
		var observable = minioClient.ListObjectsAsync(listArgs);

		foreach (var item in observable)
			files.Add(
				new
				{
					Name = item.Key,
					item.Size,
					LastModified = item.LastModifiedDateTime
				});

		return Ok(files);
	}

	// Получение конкретного файла (например, постера фильма)
	[HttpGet("files/{fileName}")]
	public async Task<IActionResult> GetFile(string fileName)
	{
		// Сначала получаем метаданные файла
		var statArgs = new StatObjectArgs()
			.WithBucket(_options.DefaultBucket)
			.WithObject(fileName);

		var stat = await minioClient.StatObjectAsync(statArgs);

		// Затем получаем сам файл
		var memoryStream = new MemoryStream();

		var getObjectArgs = new GetObjectArgs()
			.WithBucket(_options.DefaultBucket)
			.WithObject(fileName)
			.WithCallbackStream(stream => stream.CopyTo(memoryStream));

		await minioClient.GetObjectAsync(getObjectArgs);
		memoryStream.Position = 0;

		// Определяем Content-Type
		var contentType = stat.ContentType ?? "application/octet-stream";

		return File(memoryStream, contentType, fileName);
	}

	// Генерация URL для доступа к файлу
	[HttpGet("files/{fileName}/url")]
	public async Task<IActionResult> GetFileUrl(string fileName, [FromQuery] int expiry = 604800)
	{
		var presignedArgs = new PresignedGetObjectArgs()
			.WithBucket(_options.DefaultBucket)
			.WithObject(fileName)
			.WithExpiry(expiry);

		var url = await minioClient.PresignedGetObjectAsync(presignedArgs);

		return Ok(new { Url = url });
	}
}