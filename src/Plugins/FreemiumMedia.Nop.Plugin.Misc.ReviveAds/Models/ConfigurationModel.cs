using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.LeaderboardFooterReviveZoneId")]
        public string LeaderboardFooterReviveZoneId { get; set; }
        [NopResourceDisplayName("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.LeaderboardHeaderReviveZoneId")]
        public string LeaderboardHeaderReviveZoneId { get; set; }
        [NopResourceDisplayName("FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.SkyscraperReviveZoneId")]
        public string SkyscraperReviveZoneId { get; set; }
    }
}