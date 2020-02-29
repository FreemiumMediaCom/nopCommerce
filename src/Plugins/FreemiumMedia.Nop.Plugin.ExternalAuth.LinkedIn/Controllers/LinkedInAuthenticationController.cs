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
using FreemiumMedia.Nop.Plugin.ExternalAuth.LinkedIn.Models;

namespace FreemiumMedia.Nop.Plugin.ExternalAuth.LinkedIn.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class LinkedInAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly LinkedInAuthenticationSettings _linkedInAuthenticationSettings;
        private readonly INotificationService _notificationService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public LinkedInAuthenticationController(LinkedInAuthenticationSettings linkedInAuthenticationSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            INotificationService notificationService)
        {
            this._linkedInAuthenticationSettings = linkedInAuthenticationSettings;
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
                ClientSecret = _linkedInAuthenticationSettings.ClientSecret,
                ClientId = _linkedInAuthenticationSettings.ClientKeyIdentifier
            };

            return View("~/Plugins/FreemiumMedia.ExternalAuth.LinkedIn/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _linkedInAuthenticationSettings.ClientKeyIdentifier = model.ClientId;
            _linkedInAuthenticationSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_linkedInAuthenticationSettings);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public IActionResult Login(string returnUrl)
        {

            if (string.IsNullOrEmpty(_linkedInAuthenticationSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_linkedInAuthenticationSettings.ClientSecret))
                throw new NopException("LinkedIn authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "LinkedInAuthentication", new { returnUrl = returnUrl })
            };

            return Challenge(authenticationProperties, LinkedInAuthenticationDefaults.LINKED_IN_AUTHENTICATION_SCHEME);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate Facebook user
            var authenticateResult =  await this.HttpContext.AuthenticateAsync(LinkedInAuthenticationDefaults.LINKED_IN_AUTHENTICATION_SCHEME);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = LinkedInAuthenticationDefaults.PROVIDER_SYSTEM_NAME,
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