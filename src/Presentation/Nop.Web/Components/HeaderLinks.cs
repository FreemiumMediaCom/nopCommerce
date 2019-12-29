﻿using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HeaderLinksViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public HeaderLinksViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareHeaderLinksModel();
            return View(model);
        }
    }
}
