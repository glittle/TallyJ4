using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using TallyJ3.Extensions;

namespace TallyJ3.Code.Helper
{
  /// <summary>
  /// Handles two cases... associated files, random files
  /// </summary>
  public static class FileLinking
  {
    public const string RequestPath = "/client";

    /// <summary>
    /// Add CSS next to this page. Client will call at the RequestPath
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static HtmlString AddAssociatedCssTag(IRazorPage page)
    {
      if (page == null)
      {
        return null;
      }
      var version = GetVersion(page, ".css");
      if (version.HasNoContent())
      {
        return null;
      }

      return new HtmlString("<link rel='stylesheet' href='{0}.css{1}'>".FilledWith(page.Path.Replace("/Pages", RequestPath), version));
    }

    /// <summary>
    /// Add JS next to this page. Client will call at the RequestPath
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static HtmlString AddAssociatedJsTag(IRazorPage page)
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

      return new HtmlString("<script src='{0}.js{1}'></script>".FilledWith(page.Path.Replace("/Pages", RequestPath), version));
    }

    /// <summary>
    ///   Return the link to the content file, with a version number based on the timestamp.
    ///   If the path starts with ~ then the full path is needed.
    ///   Otherwise the file is in the associated wwwroot folder
    /// </summary>
    public static HtmlString CssTag(string contentFilePath, string productionNameModifier = "",
      string debuggingNameModifier = "")
    {
      var useProductionFiles = Startup.Configuration["UseProductionFiles"].AsBoolean();
      if (productionNameModifier.HasContent() || debuggingNameModifier.HasContent())
      {
        contentFilePath =
          contentFilePath.FilledWith(useProductionFiles ? productionNameModifier : debuggingNameModifier);
      }


      var version = GetVersionCode(contentFilePath);
      if (version == null)
      {
        return null;
      }

      if (contentFilePath.StartsWith("~"))
      {
        contentFilePath = contentFilePath.Substring(1);
      }

      return new HtmlString("<link rel='stylesheet' href='" + contentFilePath + "?v=" + version + "'>");
    }


    public static HtmlString JsTag(string contentFilePath, string productionNameModifier = "",
      string debuggingNameModifier = "")
    {
      var useProductionFiles = Startup.Configuration["UseProductionFiles"].AsBoolean();
      if (productionNameModifier.HasContent() || debuggingNameModifier.HasContent())
      {
        contentFilePath =
          contentFilePath.FilledWith(useProductionFiles ? productionNameModifier : debuggingNameModifier);
      }

      var version = GetVersionCode(contentFilePath);
      if (version == null)
      {
        return null;
      }

      if (contentFilePath.StartsWith("~"))
      {
        contentFilePath = contentFilePath.Substring(1);
      }

      return new HtmlString("<script src='" + contentFilePath + "?v=" + version + "'></script>");
    }

    public static string WithVersion(this string filePath)
    {
      var version = GetVersionCode(filePath);
      return filePath + (version == null ? "" : ("?v=" + version));
    }

    private static string GetVersionCode(string contentFilePath)
    {
      // TODO FUTURE: merge with the cached version of FileVersioner

      var rawPath = contentFilePath;
      if (rawPath.StartsWith("~"))
      {
        rawPath = "wwwroot\\" + rawPath.Substring(1);
      }
      else {
        rawPath = contentFilePath;
      }

      var fileInfo = Startup.Env.ContentRootFileProvider.GetFileInfo(rawPath);
      if (!fileInfo.Exists)
      {
        return null;
      }

      return fileInfo.LastModified.ToUnixTimeMilliseconds().ToString().Right(5);
    }

    private static string GetVersion(IRazorPage page, string extension)
    {
      var path = page.Path + extension;

      var context = page.ViewContext.HttpContext;
      var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
      var hostingEnvironment = context.RequestServices.GetRequiredService<IHostingEnvironment>();
      var versionProvider = new FileVersioner(hostingEnvironment.ContentRootFileProvider, cache, context.Request.Path);
      return versionProvider.GetFileVersion(path);
    }

  }





  /// <summary>
  /// Provides version hash for a specified file.
  /// </summary>
  public class FileVersioner
  {
    private const string VersionKey = "v";
    private static readonly char[] QueryStringAndFragmentTokens = new[] { '?', '#' };
    private readonly IFileProvider _fileProvider;
    private readonly IMemoryCache _cache;
    private readonly PathString _requestPathBase;

    /// <summary>
    /// Creates a new instance of <see cref="FileVersionProvider"/>.
    /// </summary>
    /// <param name="fileProvider">The file provider to get and watch files.</param>
    /// <param name="cache"><see cref="IMemoryCache"/> where versioned urls of files are cached.</param>
    /// <param name="requestPathBase">The base path for the current HTTP request.</param>
    public FileVersioner(
        IFileProvider fileProvider,
        IMemoryCache cache,
        PathString requestPathBase)
    {
      if (fileProvider == null)
      {
        throw new ArgumentNullException(nameof(fileProvider));
      }

      if (cache == null)
      {
        throw new ArgumentNullException(nameof(cache));
      }

      _fileProvider = fileProvider;
      _cache = cache;
      _requestPathBase = requestPathBase;
    }

    /// <summary>
    /// Adds version query parameter to the specified file path.
    /// </summary>
    /// <param name="path">The path of the file to which version should be added.</param>
    /// <returns>Path containing the version query string.</returns>
    /// <remarks>
    /// The version query string is appended with the key "v".
    /// </remarks>
    public string GetFileVersion(string path)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      var resolvedPath = path;

      var appendToQuery = false;
      var queryStringOrFragmentStartIndex = path.IndexOfAny(QueryStringAndFragmentTokens);
      if (queryStringOrFragmentStartIndex != -1)
      {
        // drop query and hash
        // if path has a hash, this whole versioning system may not be effective
        resolvedPath = path.Substring(0, queryStringOrFragmentStartIndex);
        appendToQuery = true;
      }

      Uri uri;
      if (Uri.TryCreate(resolvedPath, UriKind.Absolute, out uri) && !uri.IsFile)
      {
        // Don't append version if the path is absolute.
        return path;
      }

      string value;
      if (!_cache.TryGetValue(path, out value))
      {
        var cacheEntryOptions = new MemoryCacheEntryOptions();

        var fileInfo = _fileProvider.GetFileInfo(resolvedPath);

        if (!fileInfo.Exists &&
            _requestPathBase.HasValue &&
            resolvedPath.StartsWith(_requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
        {
          var requestPathBaseRelativePath = resolvedPath.Substring(_requestPathBase.Value.Length);

          fileInfo = _fileProvider.GetFileInfo(requestPathBaseRelativePath);
        }

        var relativePath = fileInfo.PhysicalPath.Replace(((PhysicalFileProvider)_fileProvider).Root, "");

        cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(relativePath));

        if (fileInfo.Exists)
        {
          value = "{0}{1}={2}".FilledWith(appendToQuery ? "&" : "?", VersionKey, GetVersionCode(fileInfo));
        }
        else
        {
          // if the file is not in the current server.
          value = null;
        }

        _cache.Set(path, value, cacheEntryOptions);
      }

      return value;
    }

    private static string GetVersionCode(IFileInfo fileInfo)
    {
      return fileInfo.LastModified.ToUnixTimeMilliseconds().ToString().Right(5);
      //using (var sha256 = CryptographyAlgorithms.CreateSHA256())
      //{
      //  using (var readStream = fileInfo.CreateReadStream())
      //  {
      //    var hash = sha256.ComputeHash(readStream);
      //    return WebEncoders.Base64UrlEncode(hash);
      //  }
      //}
    }
  }
}