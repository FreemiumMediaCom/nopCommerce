using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace FreemiumMedia.Nop.Plugin.Widgets.ReviveAds.Models
{
    public class ReviveAdsModel : BaseNopModel
    {
        #region Ctor

        public ReviveAdsModel()
        {
        }

        #endregion

        #region Properties

        public string LeaderboardFooterReviveZoneId { get; set; }
        public string LeaderboardHeaderReviveZoneId { get; set; }
        public string SkyscraperReviveZoneId { get; set; }

        #endregion
    }
}