namespace MyFilesystem.FTPFilesystem
{
    public class ImageCacheMetadataObject
    {
        public DateTime SourceLastWriteTimeUtc { get; set; }
        public DateTime CacheLastWriteTimeUtc { get; set;  }
        public string ContentType { get; set; }
        public TimeSpan CacheControlMaxAge { get; set; }
        public long ContentLength { get; set; }
    }
}
