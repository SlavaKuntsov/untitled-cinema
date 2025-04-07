namespace Minios.Services;

public interface IMinioService
{
	Task<bool> BucketExistsAsync(string bucketName);
	Task CreateBucketAsync(string bucketName);
	Task UploadFileAsync(string bucketName, string objectName, Stream data, string contentType);
	Task<Stream> GetFileAsync(string bucketName, string objectName);
	Task RemoveFileAsync(string bucketName, string objectName);
	Task<string> GetPresignedUrlAsync(string bucketName, string objectName, int expiryInSeconds = 604800);
}