using System.ComponentModel.DataAnnotations;

namespace SimpleDrive.Models
{
    public class BlobMetadata
    {
        [Key]
        public string Id { get; set; }

        public long Size { get; set; }

        public DateTime CreatedAt { get; set; }

        public string StorageType { get; set; }

    }
}
