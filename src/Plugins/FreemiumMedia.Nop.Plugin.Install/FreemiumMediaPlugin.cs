using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Services.Common;

namespace FreemiumMedia.Nop.Plugin.Install
{
    public class FreemiumMediaPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FreemiumMediaPlugin(ILocalizationService localizationService,
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
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return FreemiumMediaPluginDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            //_settingService.SaveSetting(new MeetupSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Newsletter.Message", "<strong>25% Off</strong> - Sign up for our Newsletter");
            //_localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientKeyIdentifier.Hint", "Enter your CLIENT ID. Value can be found on your LinkedIn My Apps page.");
            //_localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientSecret", "Client Secret");
            //_localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientSecret.Hint", "Enter your CLIENT Secret here. You can find it on your LinkedIn My Apps page.");
            //_localizationService.AddOrUpdatePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.Instructions", "<p>To configure authentication with LinkedIn, please follow these steps:<br/><br/><ol><li>Start by navigating to your <a href=\"https://developer.linkedin.com\" target =\"_blank\" > LinkedIn for developers</a>.</li><li>Start the process of creating a new app by pressing the button <b>\"Create app\".</b></li><li>Provide \"App name\", \"Company name\", \"App description\" and upload logo and provide whatever other information that is asked for. Don't forget to accept the legal terms at the end.</li><li> You are now navigated to the \"App settings\" screen. On the top of the page, in the top menu choose <b>\"Auth\"</b></li><li>Go down to the section for OAuth 2.0 settings and edit the redirect URLs, enter  \" https://yourdomain.com/signin-linkedin \" in that field (start with http or https). Press <b>\"Update\"</b></li><li>Under \"Application credentials\" you can see the application’s Client ID and Client Secret, copy these and paste into the corresponding fields below.</li></ol><br/><br/></p>");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            //_settingService.DeleteSetting<MeetupSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("FreemiumMedia.Newsletter.Message");
            //_localizationService.DeletePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientKeyIdentifier.Hint");
            //_localizationService.DeletePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientSecret");
            //_localizationService.DeletePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientSecret.Hint");
            //_localizationService.DeletePluginLocaleResource("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.Instructions");

            base.Uninstall();
        }

        #endregion
    }
}
