using SimpleDrive.Helpers;
using SimpleDrive.Models;
using SimpleDrive.Storage;

public class S3StorageProvider : IStorageProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _bucket;
    private readonly string _endpoint;
    private readonly AwsSigner _signer;

    public S3StorageProvider(IConfiguration config)
    {
        _httpClient = new HttpClient();

        var section = config.GetSection("S3");
        _bucket = section["Bucket"];
        _endpoint = section["Endpoint"];

        _signer = new AwsSigner(
            section["AccessKey"],
            section["SecretKey"],
            section["Region"]
        );
    }

    public async Task SaveAsync(string id, byte[] data)
    {
        var url = $"{_endpoint}/{_bucket}/{id}";
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new ByteArrayContent(data)
        };
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        _signer.SignRequest(request, "PUT", $"/{_bucket}/{id}", data);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<BlobResponse> GetAsync(string id)
    {
        var url = $"{_endpoint}/{_bucket}/{id}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        _signer.SignRequest(request, "GET", $"/{_bucket}/{id}", Array.Empty<byte>());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var file = await response.Content.ReadAsByteArrayAsync();
        var createdAt = response.Content.Headers.LastModified?.UtcDateTime ?? DateTime.UtcNow;

        return new BlobResponse
        {
            Id = id,
            Data = Convert.ToBase64String(file),
            Size = file.Length,
            CreatedAt = createdAt
        };
    }
}
