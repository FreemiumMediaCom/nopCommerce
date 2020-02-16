using System;
using System.Collections.Generic;
using System.Linq;
using FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ReviveAdsController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ReviveAdsSettings _settings;

        #endregion

        #region Ctor

        public ReviveAdsController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            ReviveAdsSettings settings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _settings = settings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            //whether user has the authority to manage configuration
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare common model
            var model = new ReviveAdsSettings
            {
                SkyscraperReviveZoneId = _settings.SkyscraperReviveZoneId,
                LeaderboardFooterReviveZoneId = _settings.LeaderboardFooterReviveZoneId,
                LeaderboardHeaderReviveZoneId = _settings.LeaderboardHeaderReviveZoneId
            };

            return View("~/Plugins/FreemiumMedia.Nop.Plugin.Widgets.ReviveAds/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ReviveAdsSettings model)
        {
            //whether user has the authority to manage configuration
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _settings.LeaderboardFooterReviveZoneId = model.LeaderboardFooterReviveZoneId;
            _settings.LeaderboardHeaderReviveZoneId = model.LeaderboardHeaderReviveZoneId;
            _settings.SkyscraperReviveZoneId = model.SkyscraperReviveZoneId;

            _settingService.SaveSetting(_settings);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}