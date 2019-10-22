using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace FreemiumMedia.Nop.Plugin.Misc.Meetup.Models
{
    public class MeetupModel : BaseNopModel
    {
        #region Ctor

        public MeetupModel()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets OAuth2 client identifier
        /// </summary>
        public string ClientKeyIdentifier { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 client secret
        /// </summary>
        public string ClientSecret { get; set; }

        public string MeetupAuthToken { get; set; }

        public string MeetupGroupName { get; set; }


        #endregion
    }
}