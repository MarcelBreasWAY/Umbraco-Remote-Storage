namespace MyFilesystem.FTPFilesystem
{
    public class FTPSettings
    {
        public string MediaFolder { get; set; } = "media";
        public string CacheFolder { get; set; } = "cache";

        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 21;
    }
}
