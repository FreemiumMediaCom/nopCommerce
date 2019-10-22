using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace FreemiumMedia.Nop.Plugin.ExternalAuth.LinkedIn.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientKeyIdentifier")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("FreemiumMedia.Plugins.ExternalAuth.LinkedIn.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}