using System.Threading.Tasks;
using FreemiumMedia.Nop.Plugin.Widgets.Statcounter;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace FreemiumMedia.Nop.Plugin.Widgets.Statcounter.Components
{
    [ViewComponent(Name = StatcounterDefaults.VIEW_COMPONENT_NAME)]
    public class WidgetsStatcounterComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;

        public WidgetsStatcounterComponent(IStoreContext storeContext, 
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
            var settings = _settingService.LoadSetting<StatcounterSettings>(_storeContext.CurrentStore.Id);
            
            return View("~/Plugins/FreemiumMedia.Nop.Plugin.Widgets.Statcounter/Views/PublicInfo.cshtml", settings);
        }
    }
}