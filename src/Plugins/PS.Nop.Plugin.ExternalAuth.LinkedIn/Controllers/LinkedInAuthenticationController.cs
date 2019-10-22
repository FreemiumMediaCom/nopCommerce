using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using PS.Nop.Plugin.ExternalAuth.LinkedIn.Models;

namespace PS.Nop.Plugin.ExternalAuth.LinkedIn.Controllers
{
    public class PSLinkedInAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly LinkedInExternalAuthSettings _linkedInExternalAuthSettings;
        private readonly INotificationService _notificationService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public PSLinkedInAuthenticationController(LinkedInExternalAuthSettings linkedInExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            INotificationService notificationService)
        {
            this._linkedInExternalAuthSettings = linkedInExternalAuthSettings;
            this._externalAuthenticationService = externalAuthenticationService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._notificationService = notificationService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _linkedInExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _linkedInExternalAuthSettings.ClientSecret
            };

            return View("~/Plugins/PS.ExternalAuth.Linkedin/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _linkedInExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _linkedInExternalAuthSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_linkedInExternalAuthSettings);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public IActionResult Login(string returnUrl)
        {

            if (string.IsNullOrEmpty(_linkedInExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_linkedInExternalAuthSettings.ClientSecret))
                throw new NopException("LinkedIn authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "PSLinkedInAuthentication", new { returnUrl = returnUrl })
            };

            return Challenge(authenticationProperties, LinkedInExternalAuthConstants.LinkedInAuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate Facebook user
            var authenticateResult =  await this.HttpContext.AuthenticateAsync(LinkedInExternalAuthConstants.LinkedInAuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = LinkedInExternalAuthConstants.ProviderSystemName,
                //AccessToken = await this.HttpContext.GetTokenAsync(FacebookDefaults.AuthenticationScheme, "oauth2_access_token"),

                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }

        #endregion
    }
}