using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace PS.Nop.Plugin.ExternalAuth.LinkedIn.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("PS.Plugins.ExternalAuth.LinkedIn.ClientKeyIdentifier")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("PS.Plugins.ExternalAuth.LinkedIn.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}