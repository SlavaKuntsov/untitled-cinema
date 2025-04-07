using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Minios.Services;

public class MinioService(IMinioClient minioClient, IOptions<MinioOptions> options)
	: IMinioService
{
	private readonly MinioOptions _options = options.Value;

	public async Task<bool> BucketExistsAsync(string bucketName)
	{
		var args = new BucketExistsArgs()
			.WithBucket(bucketName);

		return await minioClient.BucketExistsAsync(args);
	}

	public async Task CreateBucketAsync(string bucketName)
	{
		var args = new MakeBucketArgs()
			.WithBucket(bucketName);

		await minioClient.MakeBucketAsync(args);
	}

	public async Task UploadFileAsync(string bucketName, string objectName, Stream data, string contentType)
	{
		bucketName ??= _options.DefaultBucket;

		if (!await BucketExistsAsync(bucketName))
			await CreateBucketAsync(bucketName);

		var putObjectArgs = new PutObjectArgs()
			.WithBucket(bucketName)
			.WithObject(objectName)
			.WithStreamData(data)
			.WithObjectSize(data.Length)
			.WithContentType(contentType);

		await minioClient.PutObjectAsync(putObjectArgs);
	}

	public async Task<Stream> GetFileAsync(string bucketName, string objectName)
	{
		bucketName ??= _options.DefaultBucket;

		var memoryStream = new MemoryStream();

		var getObjectArgs = new GetObjectArgs()
			.WithBucket(bucketName)
			.WithObject(objectName)
			.WithCallbackStream(stream => stream.CopyTo(memoryStream));

		await minioClient.GetObjectAsync(getObjectArgs);
		memoryStream.Position = 0;

		return memoryStream;
	}

	public async Task RemoveFileAsync(string bucketName, string objectName)
	{
		bucketName ??= _options.DefaultBucket;

		var removeObjectArgs = new RemoveObjectArgs()
			.WithBucket(bucketName)
			.WithObject(objectName);

		await minioClient.RemoveObjectAsync(removeObjectArgs);
	}

	public async Task<string> GetPresignedUrlAsync(
		string bucketName,
		string objectName,
		int expiryInSeconds = 604800)
	{
		bucketName ??= _options.DefaultBucket;

		var args = new PresignedGetObjectArgs()
			.WithBucket(bucketName)
			.WithObject(objectName)
			.WithExpiry(expiryInSeconds);

		return await minioClient.PresignedGetObjectAsync(args);
	}
}