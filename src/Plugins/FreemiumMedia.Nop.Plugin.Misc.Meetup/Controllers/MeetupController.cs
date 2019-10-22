using System;
using System.Collections.Generic;
using System.Linq;
using FreemiumMedia.Nop.Plugin.Misc.Meetup.Models;
using FreemiumMedia.Nop.Plugin.Misc.Meetup.Services;
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

namespace FreemiumMedia.Nop.Plugin.Misc.Meetup.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class MeetupController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly MeetupService _meetupService;
        private readonly MeetupSettings _meetupSettings;

        #endregion

        #region Ctor

        public MeetupController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            MeetupService meetupService,
            MeetupSettings meetupSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _meetupService = meetupService;
            _meetupSettings = meetupSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            //whether user has the authority to manage configuration
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare common model
            var model = new MeetupSettings
            {
                MeetupAuthToken = _meetupSettings.MeetupAuthToken,
                MeetupGroupName = _meetupSettings.MeetupGroupName
            };

            return View("~/Plugins/FreemiumMedia.Misc.Meetup/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(MeetupSettings model)
        {
            //whether user has the authority to manage configuration
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _meetupSettings.MeetupGroupName = model.MeetupGroupName;
            _meetupSettings.MeetupAuthToken = model.MeetupAuthToken;


            _settingService.SaveSetting(_meetupSettings);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}