
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

        public bool CanAddPhysical => true;

        public void AddFile(string path, Stream stream)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            // upload a file and ensure the FTP directory is created on the server
            ftp.UploadStream(stream, path, FtpRemoteExists.Overwrite, true);
        }

        public void AddFile(string path, Stream stream, bool overrideIfExists)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            // upload a file and ensure the FTP directory is created on the server
            ftp.UploadStream(stream, path, overrideIfExists ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip, true);
        }

        public void AddFile(string path, string physicalPath, bool overrideIfExists = true, bool copy = false)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            // upload a file and ensure the FTP directory is created on the server
            ftp.UploadFile(physicalPath, path, overrideIfExists ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip, true);
        }

        public void DeleteDirectory(string path)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.DeleteDirectory(path, FtpListOption.AllFiles);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.DeleteDirectory(path, FtpListOption.AllFiles);
        }

        public void DeleteFile(string path)
        {
            path = path.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            ftp.DeleteFile(path);
        }

        public bool DirectoryExists(string path)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.DirectoryExists(path);
        }

        public bool FileExists(string path)
        {
            path = path.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.FileExists(path);
        }

        public string GetContentType(string path)
        {
            _ = new FileExtensionContentTypeProvider().TryGetContentType(path, out var contentType);
            return contentType ?? "application/octet-stream";
        }

        public DateTimeOffset GetCreated(string path)
        {
            path = path.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetObjectInfo(path).Created;
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetListing(path).Where(x => x != null && x.Type == FtpObjectType.Directory).Select(x=> x.Name);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetListing(path).Where(x => x != null && x.Type == FtpObjectType.File).Select(x => x.Name);
        }

        public IEnumerable<string> GetFiles(string path, string filter)
        {
            
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetListing(path).Where(x => x != null && x.Type == FtpObjectType.File && Regex.IsMatch(x.Name, "(\\S+?(?:" + filter.Replace("*.", string.Empty) + "))")).Select(x => x.Name);
        }

        public string GetFullPath(string path)
        {
            return path.EnsureStartsWith("/media/");
        }

        public DateTimeOffset GetLastModified(string path)
        {
            path = path.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetObjectInfo(path).Modified;
        }

        public string GetRelativePath(string fullPathOrUrl)
        {
            return fullPathOrUrl.ReplaceFirst($"ftp://{WebUtility.UrlEncode(_settings.Username)}:{WebUtility.UrlEncode(_settings.Password)}@{_settings.Host}:{_settings.Port}", string.Empty).EnsureStartsWith("/");
        }

        public long GetSize(string path)
        {
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.GetObjectInfo(path).Size;
        }

        public string GetUrl(string? path)
        {
            return path?.EnsureStartsWith("/media/");
        }

        public Stream OpenFile(string path)
        {
            path = path.ReplaceFirst("/media", string.Empty);
            using var ftp = new FtpClient(_settings.Host, _settings.Username, _settings.Password, _settings.Port);
            ftp.Connect();
            return ftp.OpenRead(path);
        }
    }
}
