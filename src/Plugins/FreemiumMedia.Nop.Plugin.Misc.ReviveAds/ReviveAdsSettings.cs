using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FreemiumMedia.Nop.Plugin.Widgets.ReviveAds
{
    public class ReviveAdsSettings : ISettings
    {
        public string LeaderboardHeaderReviveZoneId { get; set; }
        public string LeaderboardFooterReviveZoneId { get; set; }
        public string SkyscraperReviveZoneId { get; set; }
        public string Zone { get; set; }

    }
}
