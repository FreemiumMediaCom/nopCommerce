using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class FreemiumMedia_NewsletterBoxViewComponent : NopViewComponent
    {
        private readonly CustomerSettings _customerSettings;
        private readonly INewsletterModelFactory _newsletterModelFactory;

        public FreemiumMedia_NewsletterBoxViewComponent(CustomerSettings customerSettings, INewsletterModelFactory newsletterModelFactory)
        {
            _customerSettings = customerSettings;
            _newsletterModelFactory = newsletterModelFactory;
        }

public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            var model = _newsletterModelFactory.PrepareNewsletterBoxModel();
            return View(model);
        }
    }
}
