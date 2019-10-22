using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public partial class MeetupController : BasePublicController
    {
        [HttpsRequirement(SslRequirement.No)]
        public virtual async Task<IActionResult> List()
        {
            return View();
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual async Task<IActionResult> Details(string id)
        {
            return View("Details", id);
        }
    }
}