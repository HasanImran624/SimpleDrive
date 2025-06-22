namespace SimpleDrive.Models
{
    public class BlobData
    {
        public string Id { get; set; }
        public byte[] Data { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
