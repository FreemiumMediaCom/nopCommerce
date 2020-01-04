using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace FreemiumMedia.Nop.Plugin.ExternalAuth.LinkedIn.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("FreemiumMedia.Nop.Plugin.Widgets.TawkIO.WidgetCode")]
        public string WidgetCode { get; set; }
    }
}