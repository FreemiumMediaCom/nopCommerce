using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class LogoViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public LogoViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareLogoModel();
            return View(model);
        }
    }
}
