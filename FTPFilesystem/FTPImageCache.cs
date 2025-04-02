﻿using FluentFTP;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Resolvers;
using System.IO;
using System.Runtime;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;

namespace MyFilesystem.FTPFilesystem
{
    public class FTPImageCache(IOptions<FTPSettings> settings) : IImageCache
    {
        private const string cacheFolder = "cache/";
        private readonly FTPSettings _settings = settings.Value;

        public Task<IImageCacheResolver?> GetAsync(string key)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();

            if(!ftp.FileExists(cacheFolder + key + ".meta"))
            {
                return Task.FromResult<IImageCacheResolver?>(null);
            }

            ftp.DownloadBytes(out var bytes, cacheFolder + key + ".meta");

            var metadata = JsonSerializer.Deserialize<ImageCacheMetadataObject>(bytes);

            return Task.FromResult<IImageCacheResolver?>(new FTPImageCacheResolver(_settings, cacheFolder + key, metadata));
        }

        public Task SetAsync(string key, Stream stream, ImageCacheMetadata metadata)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();

            ftp.UploadStream(stream, cacheFolder + key, FtpRemoteExists.Overwrite, true);
            ftp.UploadBytes(JsonSerializer.SerializeToUtf8Bytes(metadata), cacheFolder + key + ".meta", FtpRemoteExists.Overwrite, true);
            return Task.CompletedTask;
        }
    }
}
