using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace FreemiumMedia.Nop.Plugin.Widgets.Statcounter.Models
{
    public class StatcounterModel : BaseNopModel
    {
        #region Ctor

        public StatcounterModel()
        {
        }

        #endregion

        #region Properties

        public string StatcounterCode { get; set; }

        #endregion
    }
}