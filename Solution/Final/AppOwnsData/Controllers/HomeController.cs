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
using Microsoft.Rest;

namespace AppOwnsData.Controllers {

  [Authorize]
  public class HomeController : Controller {

    private PowerBiServiceApi powerBiServiceApi;

    public HomeController(PowerBiServiceApi powerBiServiceApi) {
      this.powerBiServiceApi = powerBiServiceApi;
    }

    [AllowAnonymous]
    public IActionResult Index() {
      return View();
    }

    public async Task<IActionResult> Embed(string workspaceId) {

      Guid validWorkspaceId;
      bool workspaceIdIsValid = Guid.TryParse(workspaceId, out validWorkspaceId);

      try {
        if (workspaceIdIsValid) {
          var viewModel = await this.powerBiServiceApi.GetEmbeddedViewModel(workspaceId);
          return View(viewModel as object);
        }
        else {
          var firstWorkspace = await this.powerBiServiceApi.GetFirstWorkspace();
          if (firstWorkspace != null) {
            return RedirectToAction("Embed", "Home", new { workspaceId = firstWorkspace.Id });
          }
          else {
            throw new ApplicationException("This service principal for the Azure AD app is not a member of any workspaces.");
          }
        }
      }
      catch (HttpOperationException ex) {
        return StatusCode(500, "Unexpected HTTP exception: " + ex.Response.Content);
      }
      catch (Exception ex) {
        return StatusCode(500, "Unexpected exception: " + ex.Message);
      }
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

  }
}
