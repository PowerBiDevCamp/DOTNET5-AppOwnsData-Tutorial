using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppOwnsData.Models;
using AppOwnsData.Services;

namespace AppOwnsData.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private PowerBiServiceApi powerBiServiceApi;

        public HomeController(PowerBiServiceApi powerBiServiceApi)
        {
            this.powerBiServiceApi = powerBiServiceApi;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Embed(string workspaceId)
        {

            try
            {
                Guid guidTest = new Guid(workspaceId);
                var viewModel = await this.powerBiServiceApi.GetEmbeddedViewModel(workspaceId);
                return View(viewModel as object);
            }
            catch
            {
                var firstWorkspace = await this.powerBiServiceApi.GetFirstWorkspace();
                if (firstWorkspace == null)
                {
                    return RedirectToPage("/Error");
                }
                else
                {
                    return RedirectToPage("/Embed", null, new { workspaceId = firstWorkspace.Id });
                }


            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
