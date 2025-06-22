
using SimpleDrive.Models;

namespace SimpleDrive.Storage
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _basePath;

        public LocalStorageProvider(IConfiguration config)
        {
            _basePath = config["Storage:LocalPath"] ?? "local_data";
            Directory.CreateDirectory(_basePath);
        }
        public async Task SaveAsync(string id, byte[] data)
        {
            var path = Path.Combine(_basePath, id);
            await File.WriteAllBytesAsync(path, data);
        }

        public async Task<BlobResponse> GetAsync(string id)
        {
            var path = Path.Combine(_basePath, id);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Blob with id '{id}' not found.");
            var stroredFile = await File.ReadAllBytesAsync(path);
            return new BlobResponse
            {
                Id = id,
                Data = Convert.ToBase64String(stroredFile),
                Size = stroredFile.Length
            };
        }

    }
}
