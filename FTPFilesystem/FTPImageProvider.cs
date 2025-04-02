using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using System.Configuration;

namespace MyFilesystem.FTPFilesystem
{
    public class FTPImageProvider(IOptions<FTPSettings> settings, FormatUtilities formatUtilities) : IImageProvider
    {
        private readonly string _relativeUrlPrefix = "/media";

        public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.CommandOnly;
        public Func<HttpContext, bool> Match { get; set; } = _ => true;

        private readonly FTPSettings _settings = settings.Value;
        private readonly FormatUtilities _formatUtilities = formatUtilities;

        public Task<IImageResolver?> GetAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return Task.FromResult<IImageResolver?>(new FTPImageResolver(_settings, context.Request.Path));
        }

        public bool IsValidRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Request.Path.StartsWithSegments(_relativeUrlPrefix, StringComparison.InvariantCultureIgnoreCase)
                   && _formatUtilities.TryGetExtensionFromUri(context.Request.GetDisplayUrl(), out _);
        }
    }
}
