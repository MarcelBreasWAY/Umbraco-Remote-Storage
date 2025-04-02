using FluentFTP;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Resolvers;

namespace MyFilesystem.FTPFilesystem
{
    public class FTPImageResolver(FTPSettings settings, string filePath) : IImageResolver
    {
        private readonly string _relativeUrlPrefix = "/media/";

        private readonly FTPSettings _settings = settings;
        private readonly string _mediaFolder = settings.MediaFolder.EnsureStartsWith("/");

        public Task<ImageMetadata> GetMetaDataAsync()
        {
            var path = filePath.ReplaceFirst(_relativeUrlPrefix, "/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();

            if (!ftp.FileExists(_mediaFolder + path))
            {
                return Task.FromResult(new ImageMetadata(DateTime.Now, TimeSpan.MinValue, 0));
            }
            var metadata = ftp.GetObjectInfo(_mediaFolder + path);
            return Task.FromResult(new ImageMetadata(metadata.Modified, TimeSpan.FromMinutes(2), metadata.Size));
        }

        public Task<Stream> OpenReadAsync()
        {
            var path = filePath.ReplaceFirst(_relativeUrlPrefix, "/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return Task.FromResult(ftp.OpenRead(_mediaFolder + path));
        }
    }
}
