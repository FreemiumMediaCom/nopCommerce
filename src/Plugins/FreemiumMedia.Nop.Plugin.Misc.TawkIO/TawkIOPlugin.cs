using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace FreemiumMedia.Nop.Plugin.Widgets.TawkIO
{
    public class TawkIOPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public TawkIOPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HomepageBottom, PublicWidgetZones.BlogPostPageBottom, PublicWidgetZones.ContactUsBottom };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/TawkIO/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return TawkIODefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new TawkIOSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.TawkIO.WidgetCode", "Widget Code");
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.TawkIO.Instructions", "<p>To configure Tawk.IO, please follow these steps:<br/><br/><ol><li>Copy and Paste your 'Widget code' from Tawk.IO and save</li></ol><br/><br/></p>");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<TawkIOSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.TawkIO.WidgetCode");
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.TawkIO.Instructions");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}
