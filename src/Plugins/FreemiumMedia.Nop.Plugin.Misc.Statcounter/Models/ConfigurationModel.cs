using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace FreemiumMedia.Nop.Plugin.Widgets.Statcounter.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("FreemiumMedia.Nop.Plugin.Widgets.Adsense.StatcounterCode")]
        public string StatcounterCode { get; set; }
    }
}