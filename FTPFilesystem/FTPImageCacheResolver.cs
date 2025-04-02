using FluentFTP;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Resolvers;

namespace MyFilesystem.FTPFilesystem
{
    /// <summary>
    public class FTPImageCacheResolver(FTPSettings settings, string cacheKey, ImageCacheMetadataObject metadata) : IImageCacheResolver
    {
        private readonly FTPSettings _settings = settings;
        private readonly string _cacheKey = cacheKey;
        private readonly ImageCacheMetadataObject _metadata = metadata;

        public Task<ImageCacheMetadata> GetMetaDataAsync()
        {
            return Task.FromResult(new ImageCacheMetadata(
                _metadata.SourceLastWriteTimeUtc,
                _metadata.CacheLastWriteTimeUtc,
                _metadata.ContentType,
                _metadata.CacheControlMaxAge,
                _metadata.ContentLength
            ));
        }

        public Task<Stream> OpenReadAsync()
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return Task.FromResult(ftp.OpenRead(_cacheKey));
        }
    }
}
