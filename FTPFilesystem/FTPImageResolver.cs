using FluentFTP;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using NPoco.RowMappers;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Resolvers;
using System.Runtime;

namespace MyFilesystem.FTPFilesystem
{
    public class FTPImageResolver(FTPSettings settings, string filePath) : IImageResolver
    {
        private readonly FTPSettings _settings = settings;

        public Task<ImageMetadata> GetMetaDataAsync()
        {
            var path = filePath.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();

            if (!ftp.FileExists(path))
            {
                return Task.FromResult(new ImageMetadata(DateTime.Now, TimeSpan.MinValue, 0));
            }
            var metadata = ftp.GetObjectInfo(path);
            return Task.FromResult(new ImageMetadata(metadata.Modified, TimeSpan.FromMinutes(2), metadata.Size));
        }

        public Task<Stream> OpenReadAsync()
        {
            var path = filePath.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return Task.FromResult(ftp.OpenRead(path));
        }
    }
}
