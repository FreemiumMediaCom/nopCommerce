using System.Threading.Tasks;
using FreemiumMedia.Nop.Plugin.Widgets.TawkIO;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace FreemiumMedia.Nop.Plugin.Widgets.TawkIO.Components
{
    [ViewComponent(Name = TawkIODefaults.VIEW_COMPONENT_NAME)]
    public class WidgetsTawkIOComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;

        public WidgetsTawkIOComponent(IStoreContext storeContext, 
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
            var tawkIOSettings = _settingService.LoadSetting<TawkIOSettings>(_storeContext.CurrentStore.Id);
            
            return View("~/Plugins/FreemiumMedia.Nop.Plugin.Widgets.TawkIO/Views/PublicInfo.cshtml", tawkIOSettings);
        }
    }
}