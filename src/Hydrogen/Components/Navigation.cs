using Hydrogen.Core.Domain.Multitenancy;
using Microsoft.AspNetCore.Mvc;


namespace Hydrogen.Components
{
    public class Navigation : ViewComponent
    {
        private readonly ApplicationTenant _tenant;
        public Navigation(ApplicationTenant tenant)
        {
            _tenant = tenant;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
