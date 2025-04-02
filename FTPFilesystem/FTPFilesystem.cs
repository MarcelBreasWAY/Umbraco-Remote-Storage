
using FluentFTP;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;

namespace MyFilesystem.FTPFilesystem
{
    public class FTPFilesystem(IOptions<FTPSettings> settings) : IFTPFilesystem
    {
        private readonly FTPSettings _settings = settings.Value;
        private readonly string _mediaFolder = settings.Value.MediaFolder.EnsureStartsWith("/");
        private readonly string _relativeUrlPrefix = "/media/";
        public bool CanAddPhysical => true;

        public void AddFile(string path, Stream stream)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.UploadStream(stream, _mediaFolder + path, FtpRemoteExists.Overwrite, true);
        }

        public void AddFile(string path, Stream stream, bool overrideIfExists)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.UploadStream(stream, _mediaFolder + path, overrideIfExists ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip, true);
        }

        public void AddFile(string path, string physicalPath, bool overrideIfExists = true, bool copy = false)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.UploadFile(physicalPath, _mediaFolder + path, overrideIfExists ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip, true);
        }

        public void DeleteDirectory(string path)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.DeleteDirectory(_mediaFolder + path, FtpListOption.AllFiles);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.DeleteDirectory(_mediaFolder + path, FtpListOption.AllFiles);
        }

        public void DeleteFile(string path)
        {
            path = path.ReplaceFirst(_relativeUrlPrefix, "/").EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.DeleteFile(_mediaFolder + path);
        }

        public bool DirectoryExists(string path)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.DirectoryExists(_mediaFolder + path);
        }

        public bool FileExists(string path)
        {
            path = path.ReplaceFirst(_relativeUrlPrefix, "/").EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.FileExists(_mediaFolder + path);
        }

        public DateTimeOffset GetCreated(string path)
        {
            path = path.ReplaceFirst(_relativeUrlPrefix, "/").EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetObjectInfo(_mediaFolder + path).Created;
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetListing(_mediaFolder + path).Where(x => x != null && x.Type == FtpObjectType.Directory).Select(x=> x.Name);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetListing(_mediaFolder + path).Where(x => x != null && x.Type == FtpObjectType.File).Select(x => x.Name);
        }

        public IEnumerable<string> GetFiles(string path, string filter)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetListing(_mediaFolder + path).Where(x => x != null && x.Type == FtpObjectType.File && Regex.IsMatch(x.Name, "(\\S+?(?:" + filter.Replace("*.", string.Empty) + "))")).Select(x => x.Name);
        }

        public string GetFullPath(string path)
        {
            return path.ToLower().EnsureStartsWith(_relativeUrlPrefix);
        }

        public DateTimeOffset GetLastModified(string path)
        {
            path = path.ReplaceFirst(_relativeUrlPrefix, "/").EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetObjectInfo(_mediaFolder + path).Modified;
        }

        public string GetRelativePath(string fullPathOrUrl)
        {
            return fullPathOrUrl.ReplaceFirst($"ftp://{WebUtility.UrlEncode(_settings.Username)}:{WebUtility.UrlEncode(_settings.Password)}@{_settings.Host}:{_settings.Port}", string.Empty).EnsureStartsWith("/");
        }

        public long GetSize(string path)
        {
            path = path.EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetObjectInfo(_mediaFolder + path).Size;
        }

        public string GetUrl(string? path)
        {
            return path?.EnsureStartsWith(_relativeUrlPrefix) ?? string.Empty;
        }

        public Stream OpenFile(string path)
        {
            path = path.ReplaceFirst(_relativeUrlPrefix, "/").EnsureStartsWith("/");
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.OpenRead(_mediaFolder + path);
        }

        public string GetContentType(string path)
        {
            _ = new FileExtensionContentTypeProvider().TryGetContentType(path, out var contentType);
            return contentType ?? "application/octet-stream";
        }
    }
}
