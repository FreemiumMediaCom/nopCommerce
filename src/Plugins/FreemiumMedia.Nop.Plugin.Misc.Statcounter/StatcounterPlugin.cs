using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace FreemiumMedia.Nop.Plugin.Widgets.Statcounter
{
    public class StatcounterPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public StatcounterPlugin(ILocalizationService localizationService,
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
            return new List<string> { PublicWidgetZones.Footer };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Statcounter/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return StatcounterDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new StatcounterSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.Statcounter.StatcounterCode", "Statcounter Code");
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.Statcounter.Instructions", "<p>To configure Statcounter, please follow these steps:<br/><br/><ol><li>Copy and Paste your 'Statcounter Code' and save</li></ol><br/><br/></p>");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<StatcounterSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.Statcounter.AdsenseUrl");
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.Statcounter.Instructions");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}
