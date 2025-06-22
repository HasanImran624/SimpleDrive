using SimpleDrive.Models;

namespace SimpleDrive.Storage
{
    public interface IStorageProvider
    {
        public Task SaveAsync(string id, byte[] data);
        public Task<BlobResponse> GetAsync(string id);
    }
}
