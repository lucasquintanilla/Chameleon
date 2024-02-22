using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Voxed.WebApp.Views.Shared.Components.Nav
{
    public class NavViewComponent : ViewComponent
    {
        public static string Name = "Nav";

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
