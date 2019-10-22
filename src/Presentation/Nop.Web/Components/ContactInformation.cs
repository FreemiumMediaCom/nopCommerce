using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ContactInformationViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public ContactInformationViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareContactInformationModel();
            return View(model);
        }
    }
}
