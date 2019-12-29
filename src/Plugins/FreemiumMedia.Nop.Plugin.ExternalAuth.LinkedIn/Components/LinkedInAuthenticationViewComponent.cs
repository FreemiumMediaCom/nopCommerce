using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace FreemiumMedia.Nop.Plugin.ExternalAuth.LinkedIn.Components
{
    [ViewComponent(Name = LinkedInAuthenticationDefaults.VIEW_COMPONENT_NAME)]
    public class LinkedInAuthenticationViewComponent : NopViewComponent
    {
        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return View("~/Plugins/FreemiumMedia.ExternalAuth.LinkedIn/Views/PublicInfo.cshtml");
        }
    }
}