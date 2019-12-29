using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class AdminHeaderLinksViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public AdminHeaderLinksViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareAdminHeaderLinksModel();
            return View(model);
        }
    }
}
