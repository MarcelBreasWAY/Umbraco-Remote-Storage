using Microsoft.Extensions.DependencyInjection.Extensions;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Umbraco.Cms.Infrastructure.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace MyFilesystem.FTPFilesystem
{
    public static class FTPFileSystemExtensions
    {
        public static IUmbracoBuilder AddFtpFileSystem(this IUmbracoBuilder builder)
        {
            builder.Services
                .AddOptions<FTPSettings>()
                .BindConfiguration($"FTPSettings");

            builder.Services.TryAddSingleton<FTPFilesystemMiddleware>();
            builder.Services.TryAddSingleton<IFTPFilesystem, FTPFilesystem>();
            builder.Services.AddUnique<IImageCache, FTPImageCache>();
            builder.Services.AddImageSharp().ClearProviders().AddProvider<FTPImageProvider>().SetCache<FTPImageCache>();

            builder.SetMediaFileSystem(provider => provider.GetRequiredService<IFTPFilesystem>());

            return builder;
        }

        public static IUmbracoApplicationBuilderContext UseFtpFileSystem(this IUmbracoApplicationBuilderContext builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            UseFtpFileSystem(builder.AppBuilder);

            return builder;
        }

        public static IApplicationBuilder UseFtpFileSystem(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseMiddleware<FTPFilesystemMiddleware>();

            return app;
        }
    }
}
