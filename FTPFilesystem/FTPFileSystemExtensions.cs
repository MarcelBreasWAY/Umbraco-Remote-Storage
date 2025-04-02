using FluentFTP.Rules;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using Umbraco.Cms.Infrastructure.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace MyFilesystem.FTPFilesystem
{
    public static class FTPFileSystemExtensions
    {
        public static IUmbracoBuilder AddFtpFileSystem(this IUmbracoBuilder builder)
        {
            builder.Services
                .AddOptions<FTPSettings>()
                .BindConfiguration($"FTPSettings");

            builder.Services.TryAddSingleton<IFTPFilesystem, FTPFilesystem>();
            builder.Services.AddUnique<IImageCache, FTPImageCache>();
            builder.Services.AddImageSharp().ClearProviders().AddProvider<FTPImageProvider>().SetCache<FTPImageCache>();

            builder.SetMediaFileSystem(provider => provider.GetRequiredService<IFTPFilesystem>());

            return builder;
        }
    }
}
