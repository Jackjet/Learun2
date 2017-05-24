using System.Web.Mvc;

namespace LeaRun.WebApp.Areas.FYModule
{
    public class FYModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FYModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FYModule_default",
                "FYModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
