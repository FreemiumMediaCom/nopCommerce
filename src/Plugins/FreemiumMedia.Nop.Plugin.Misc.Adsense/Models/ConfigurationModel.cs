using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace FreemiumMedia.Nop.Plugin.Widgets.Adsense.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("FreemiumMedia.Nop.Plugin.Widgets.Adsense.AdsenseUrl")]
        public string AdsenseUrl { get; set; }
    }
}