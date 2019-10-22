using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Meetup.NetStandard;
using Meetup.NetStandard.Response.Events;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace FreemiumMedia.Nop.Plugin.Misc.Meetup.Services
{
    /// <summary>
    /// Represents UPS service
    /// </summary>
    public class MeetupService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly MeetupSettings _settings;

        #endregion

        #region Ctor

        public MeetupService(ILocalizationService localizationService,
            ILogger logger,
            IWorkContext workContext,
            MeetupSettings meetupSettings)
        {
            _localizationService = localizationService;
            _logger = logger;
            _workContext = workContext;
            _settings = meetupSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get UPS code of enum value
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>UPS code</returns>
        public async Task<Event[]> GetEvents()
        {
            var client = MeetupClient.WithApiToken(_settings.MeetupAuthToken);
            var response = await client.Events.For(_settings.MeetupGroupName);
            var data = response.Data;

            return data;
        }

        public async Task<Event> GetEvent(string eventId)
        {
            var client = MeetupClient.WithApiToken(_settings.MeetupAuthToken);
            var response = await client.Events.For(_settings.MeetupGroupName);
            var data = response.Data;
            var item = data.FirstOrDefault(x => x.Id.Equals(eventId));

            return item;
        }

        #endregion
    }
}