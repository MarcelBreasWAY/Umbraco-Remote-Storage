using Umbraco.Cms.Core.IO;

namespace MyFilesystem.FTPFilesystem
{
    public interface IFTPFilesystem : IFileSystem
    {
        string GetContentType(string path);
    }
}
