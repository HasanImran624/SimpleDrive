using Microsoft.EntityFrameworkCore;
using SimpleDrive.Data;
using SimpleDrive.Models;
using SimpleDrive.Storage;

namespace SimpleDrive.Services
{
    public class BlobService
    {
        private readonly IStorageProvider _storageProvider;
        private readonly BlobDataContext _context;

        public BlobService(IStorageProvider storageProvider, BlobDataContext context)
        {
            _storageProvider = storageProvider;
            _context = context;
        }

        public async Task StoreBlobAsync(string id, string base64)
        {
            byte[] data = null;
            try
            {
                data = Convert.FromBase64String(base64);
            }
            catch
            {
                throw new Exception("Invalid base64");
            }

            await _storageProvider.SaveAsync(id, data);

            var metadata = new BlobMetadata
            {
                Id = id,
                Size = data.Length,
                CreatedAt = DateTime.UtcNow,
                StorageType = _storageProvider.GetType().Name,
            };

            _context.BlobMetadata.Add(metadata);
            await _context.SaveChangesAsync();

        }

        public async Task<BlobResponse> GetBlobAsync(string id)
        {

           return await _storageProvider.GetAsync(id);
     
        }
    }
}
