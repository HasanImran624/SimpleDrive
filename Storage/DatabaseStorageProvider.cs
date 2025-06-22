
using SimpleDrive.Data;
using SimpleDrive.Models;

namespace SimpleDrive.Storage
{
    public class DatabaseStorageProvider : IStorageProvider
    {
        private readonly BlobDataContext _context;

        public DatabaseStorageProvider(BlobDataContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(string id, byte[] data)
        {
            var entity = new BlobData
            {
                Id = id,
                Data = data,
                CreatedAt = DateTime.UtcNow

            };

            _context.BlobData.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<BlobResponse> GetAsync(string id)
        {
            var entity = await _context.BlobData.FindAsync(id);

            if (entity == null)
                throw new FileNotFoundException("Blob not found in DB.");

            return new BlobResponse
            {
                Id = entity.Id,
                Data = Convert.ToBase64String(entity.Data),
                CreatedAt = entity.CreatedAt,
                Size = entity.Data.Length
            };
        }

    }
}
