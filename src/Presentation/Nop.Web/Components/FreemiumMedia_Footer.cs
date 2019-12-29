using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class FreemiumMedia_FooterViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public FreemiumMedia_FooterViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareFooterModel();
            return View(model);
        }
    }
}
