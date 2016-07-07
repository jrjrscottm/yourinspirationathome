using Microsoft.AspNetCore.Mvc;
using Hydrogen.Core.Domain.Multitenancy;
namespace Hydrogen.Components
{
    public class ThemeStyle: ViewComponent
    {
        private readonly ApplicationTenant _tenant;

        public ThemeStyle(ApplicationTenant tenant)
        {
            _tenant = tenant;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
