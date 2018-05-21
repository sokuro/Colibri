using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Middleware
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseNodeModules(this IApplicationBuilder app, string root)
        {
            // override the default /wwwroot Folder
            var options = new StaticFileOptions();

            // define the new Path
            options.RequestPath = "/node_modules";

            // define the root Path
            var path = Path.Combine(root, "node_modules");

            // define the new FileProvider
            var fileProvider = new PhysicalFileProvider(path);

            // define the FileProvider with the combined Path
            options.FileProvider = fileProvider;

            // use this Options in the Static File Middleware
            app.UseStaticFiles(options);

            return app;
        }
    }
}
