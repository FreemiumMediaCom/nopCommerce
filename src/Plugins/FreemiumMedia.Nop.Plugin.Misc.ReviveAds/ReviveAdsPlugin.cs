using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace FreemiumMedia.Nop.Plugin.Widgets.ReviveAds
{
    public class ReviveAdsPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public ReviveAdsPlugin(ILocalizationService localizationService,
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
            return new List<string> { PublicWidgetZones.AdvertisingFooter, PublicWidgetZones.AdvertisingHeader, PublicWidgetZones.AdvertisingSide };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ReviveAds/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return ReviveAdsDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new ReviveAdsSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.LeaderboardHeaderReviveZoneId", "Leaderboard Header Zone ID");
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.LeaderboardFooterReviveZoneId", "Leaderboard Footer Zone ID");
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.SkyscraperReviveZoneId", "Skyscraper Zone ID");
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.Instructions", "<p>To configure, please follow these steps:<br /><br /><ol><li>Copy and Paste your 'Zone Ids' and save</li></ol><br /><br /></p>");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ReviveAdsSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.LeaderboardHeaderReviveZoneId");
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.LeaderboardFooterReviveZoneId");
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.SkyscraperReviveZoneId");
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.Instructions");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}
