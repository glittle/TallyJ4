using RazorLight;
using System.Configuration;

namespace TallyJ3.Code.Helpers
{
  public class MvcViewRenderer
  {
    //static TemplateServiceConfiguration config;

    public static string RenderRazorViewToString(string pathToView, object model = null)
    {
      //var path = HostingEnvironment.MapPath(pathToView);
      //if (path == null)
      //{
      //  return "";
      //}

      pathToView = pathToView.Replace("~", "");

      //if (config == null || ConfigurationManager.AppSettings["Environment"] == "Dev")
      //{
      //  config = new TemplateServiceConfiguration();
      //  config.TemplateManager = new ResolvePathTemplateManager(new[] { HostingEnvironment.MapPath("~") });
      //}
      var engine = new RazorLightEngineBuilder()
                .UseFilesystemProject("~")
                .UseMemoryCachingProvider()
                .Build();

      var body = engine.CompileRenderAsync(pathToView, model).Result;

      return body;
    }
  }
}