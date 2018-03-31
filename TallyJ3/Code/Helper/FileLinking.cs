using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallyJ3.Extensions;

namespace TallyJ3.Code.Helper
{
    public static class FileLinking
    {
        public const string RequestPath = "/client";

        public static HtmlString AddAssociatedCss(IRazorPage page)
        {
            if (page == null) {
                return null;
            }
            var version = GetVersion(page, ".css");
            if (version.HasNoContent())
            {
                return null;
            }

            return new HtmlString("<link rel='stylesheet' href='" + page.Path.Replace("/Pages", RequestPath) + ".css?v=" + version + "'>");
        }

        public static HtmlString AddAssociatedJs(IRazorPage page)
        {
            if (page == null)
            {
                return null;
            }
            var version = GetVersion(page, ".js");
            if (version.HasNoContent())
            {
                return null;
            }

            return new HtmlString("<script src='" + page.Path.Replace("/Pages", RequestPath) + ".js?v=" + version + "'></script>");
        }


        private static string GetVersion(IRazorPage page, string extension)
        {
            var provider = Startup.Env.ContentRootFileProvider;
            var basePath = page.Path;

            var info = provider.GetFileInfo(basePath + extension);

            if (!info.Exists)
            {
                return null;
            }

            return info.LastModified.ToUnixTimeMilliseconds().ToString().Right(5);
        }

    }
}
