using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace kpm_csharp_webform.Controllers
{
  public class HomeController : Controller
  {
    public string Index()
    {
      DotNetEnv.Env.Load();
      string test = "<div>" + (string)System.Environment.GetEnvironmentVariable("TEST") + "</div>";
      return test; //(IActionResult)
                   // return View();
    }

    public string Welcome(string name, int numTimes = 1)
    {
      return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}");
    }
  }
}
