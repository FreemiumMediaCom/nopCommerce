using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using PS.Nop.Plugin.ExternalAuth.LinkedIn;

namespace PS.Nop.Plugin.ExternalAuth.LinkedIn.Components
{
    [ViewComponent(Name = LinkedInExternalAuthConstants.ViewComponentName)]
    public class LinkedInAuthenticationViewComponent : NopViewComponent
    {
        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/PS.ExternalAuth.linkedIn/Views/PublicInfo.cshtml");
        }
    }
}