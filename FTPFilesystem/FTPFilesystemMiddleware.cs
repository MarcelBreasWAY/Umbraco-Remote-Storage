
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net;
using Umbraco.Cms.Core.Configuration.Models;

namespace MyFilesystem.FTPFilesystem
{
    public class FTPFilesystemMiddleware(IOptions<GlobalSettings> globalSettings, IFTPFilesystem filesystem) : IMiddleware
    {
        private GlobalSettings _globalSettings = globalSettings.Value;
        private IFTPFilesystem _filesystem = filesystem;

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (next == null) throw new ArgumentNullException(nameof(next));

            return HandleResolveMediaAsync(context, next);
        }

        private async Task HandleResolveMediaAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            var response = context.Response;
            var mediaUrlPrefix = _globalSettings.UmbracoMediaPath.ReplaceFirst("~/", "/");

            if (!context.Request.Path.StartsWithSegments(mediaUrlPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            string ftpPath = context.Request.Path.Value?.Substring(mediaUrlPrefix.Length);

            if (ftpPath == null || !_filesystem.FileExists(ftpPath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var fileStream = _filesystem.OpenFile(ftpPath);
            var contentType = _filesystem.GetContentType(ftpPath);

            var responseHeaders = response.GetTypedHeaders();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = contentType;
            responseHeaders.ContentLength = fileStream.Length;
            responseHeaders.Append(HeaderNames.AcceptRanges, "bytes");

            fileStream.CopyTo(response.Body);


        }
    }
}
