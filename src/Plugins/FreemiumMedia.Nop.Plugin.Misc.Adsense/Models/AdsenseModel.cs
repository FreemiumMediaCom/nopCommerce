using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace FreemiumMedia.Nop.Plugin.Widgets.Adsense.Models
{
    public class AdsenseModel : BaseNopModel
    {
        #region Ctor

        public AdsenseModel()
        {
        }

        #endregion

        #region Properties

        public string AdsenseUrl { get; set; }

        #endregion
    }
}