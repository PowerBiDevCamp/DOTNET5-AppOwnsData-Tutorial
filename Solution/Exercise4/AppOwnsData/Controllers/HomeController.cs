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

        public async Task<IActionResult> Embed() {

            // replace these two GUIDs with the workspace ID and report ID you recorded earlier
            Guid workspaceId = new Guid("11111111-1111-1111-1111-111111111111");
            Guid reportId = new Guid("22222222-2222-2222-2222-222222222222");

            var viewModel = await powerBiServiceApi.GetReport(workspaceId, reportId);

            return View(viewModel);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
