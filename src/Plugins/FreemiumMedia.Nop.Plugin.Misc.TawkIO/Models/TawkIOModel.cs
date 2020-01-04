using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace FreemiumMedia.Nop.Plugin.Widgets.TawkIO.Models
{
    public class TawkIOModel : BaseNopModel
    {
        #region Ctor

        public TawkIOModel()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Widget Code
        /// </summary>
        public string WidgetCode { get; set; }

        #endregion
    }
}