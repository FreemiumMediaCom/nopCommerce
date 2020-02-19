using System.Threading.Tasks;
using FreemiumMedia.Nop.Plugin.Widgets.Adsense;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace FreemiumMedia.Nop.Plugin.Widgets.Adsense.Components
{
    [ViewComponent(Name = AdsenseDefaults.VIEW_COMPONENT_NAME)]
    public class WidgetsAdsenseComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;

        public WidgetsAdsenseComponent(IStoreContext storeContext, 
            ISettingService settingService, 
            IWorkContext workContext)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _workContext = workContext;
        }

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var settings = _settingService.LoadSetting<AdsenseSettings>(_storeContext.CurrentStore.Id);
            
            return View("~/Plugins/FreemiumMedia.Nop.Plugin.Widgets.Adsense/Views/PublicInfo.cshtml", settings);
        }
    }
}