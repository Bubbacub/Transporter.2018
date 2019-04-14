namespace Imd.Transporter.Viewer.Controllers
{
    using System.Reflection;
    using System.Web.Http;

    [Route("api/banner")]
    public class BannerController : ApiController
    {
        [HttpGet]
        [Route("api/banner/buildNumber")]
        public string GetBuildNumber()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
