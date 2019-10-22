using System.Threading.Tasks;
using FreemiumMedia.Nop.Plugin.Misc.Meetup;
using FreemiumMedia.Nop.Plugin.Misc.Meetup.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Components;

namespace FreemiumMedia.Nop.Plugin.Misc.Meetup.Components
{
    [ViewComponent(Name = MeetupDefaults.VIEW_COMPONENT_NAME)]
    public class MeetupComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly MeetupService _meetupService;

        public MeetupComponent(IWorkContext workContext,
                  MeetupService meetupService)
        {
            _workContext = workContext;
            _meetupService = meetupService;
        }
        
        /// <summary>
                 /// Invoke view component
                 /// </summary>
                 /// <param name="widgetZone">Widget zone name</param>
                 /// <param name="additionalData">Additional data</param>
                 /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync(string eventId)
        {
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                var eventItem = await _meetupService.GetEvent(eventId);
                return View("~/Plugins/FreemiumMedia.Misc.Meetup/Views/EventDetail.cshtml", eventItem);
            }
            else
            {
                var events = await _meetupService.GetEvents();
                return View("~/Plugins/FreemiumMedia.Misc.Meetup/Views/EventList.cshtml", events);
            }
        }
    }
}