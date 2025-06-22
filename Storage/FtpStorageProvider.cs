using FluentFTP;
using Microsoft.Extensions.Configuration;
using SimpleDrive.Models;
using System.Net;
using System.Text;

namespace SimpleDrive.Storage
{
    public class FtpStorageProvider : IStorageProvider
    {
        private readonly FtpClient _ftpClient;
        private readonly string _basePath;

        public FtpStorageProvider(IConfiguration config)
        {
            var ftpSection = config.GetSection("Ftp");
            var host = ftpSection["Host"];
            var username = ftpSection["Username"];
            var password = ftpSection["Password"];
            _basePath = ftpSection["BasePath"] ?? "/";
            _ftpClient = new FtpClient(host, new NetworkCredential(username, password));
           
            _ftpClient.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            _ftpClient.Config.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            _ftpClient.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
            _ftpClient.Config.ValidateAnyCertificate = true;
            _ftpClient.Connect();

            _ftpClient = new FtpClient(host, new NetworkCredential(username, password));
            //_ftpClient.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            _ftpClient.Connect();
        }

        public async Task SaveAsync(string id, byte[] data)
        {
            var remotePath = $"{_basePath}/{id}";
            using var stream = new MemoryStream(data);
            _ftpClient.UploadStream(stream, remotePath, FtpRemoteExists.Overwrite);

        }

        public async Task<BlobResponse> GetAsync(string id)
        {
            var remotePath = $"{_basePath}/{id}";
            using var stream = new MemoryStream();
            var success = _ftpClient.DownloadStream(stream, remotePath);

            if (!success)
                throw new FileNotFoundException("File not found on FTP server.", id);

            var data = stream.ToArray();
            return new BlobResponse
            {
                Id = id,
                Data = Convert.ToBase64String(data),
                Size = data.Length,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
