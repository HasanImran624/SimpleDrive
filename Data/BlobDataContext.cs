using Microsoft.EntityFrameworkCore;
using SimpleDrive.Models;
using System.Collections.Generic;

namespace SimpleDrive.Data
{
    public class BlobDataContext : DbContext
    {
        public BlobDataContext(DbContextOptions<BlobDataContext> options) : base(options) { }

        public DbSet<BlobData> BlobData { get; set; }

        public DbSet<BlobMetadata> BlobMetadata { get; set; }
        
    }
}
