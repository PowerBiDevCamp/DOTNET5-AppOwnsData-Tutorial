using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;

namespace AppOwnsData.Services {

  public class EmbeddedReportViewModel {
    public string Id;
    public string Name;
    public string EmbedUrl;
    public string Token;
  }

  public class PowerBiServiceApi {

    private ITokenAcquisition tokenAcquisition { get; }
    private string urlPowerBiServiceApiRoot { get; }

    public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition) {
      this.urlPowerBiServiceApiRoot = configuration["PowerBi:ServiceRootUrl"];
      this.tokenAcquisition = tokenAcquisition;
    }

    public const string powerbiApiDefaultScope = "https://analysis.windows.net/powerbi/api/.default";

    public string GetAccessToken() {
      return this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiApiDefaultScope).Result;
    }

    public PowerBIClient GetPowerBiClient() {
      var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");
      return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
    }

    public async Task<EmbeddedReportViewModel> GetReport(Guid WorkspaceId, Guid ReportId) {

      PowerBIClient pbiClient = GetPowerBiClient();

      // call to Power BI Service API to get embedding data
      var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, ReportId);

      // generate read-only embed token for the report
      var datasetId = report.DatasetId;
      var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, datasetId);
      var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(WorkspaceId, ReportId, tokenRequest);
      var embedToken = embedTokenResponse.Token;

      // return report embedding data to caller
      return new EmbeddedReportViewModel {
        Id = report.Id.ToString(),
        EmbedUrl = report.EmbedUrl,
        Name = report.Name,
        Token = embedToken
      };
    }

    public async Task<string> GetEmbeddedViewModel(string appWorkspaceId = "") {

      PowerBIClient pbiClient = GetPowerBiClient();

      Object viewModel;

      Guid workspaceId = new Guid(appWorkspaceId);
      var workspaces = (await pbiClient.Groups.GetGroupsAsync()).Value;
      var currentWorkspace = workspaces.First((workspace) => workspace.Id == workspaceId);
      var datasets = (await pbiClient.Datasets.GetDatasetsInGroupAsync(workspaceId)).Value;
      var reports = (await pbiClient.Reports.GetReportsInGroupAsync(workspaceId)).Value;

      IList<GenerateTokenRequestV2Dataset> datasetRequests = new List<GenerateTokenRequestV2Dataset>();
      foreach (var dataset in datasets) {
        datasetRequests.Add(new GenerateTokenRequestV2Dataset(dataset.Id));
      };

      IList<GenerateTokenRequestV2Report> reportRequests = new List<GenerateTokenRequestV2Report>();
      foreach (var report in reports) {
        reportRequests.Add(new GenerateTokenRequestV2Report(report.Id, allowEdit: true));
      };


      IList<GenerateTokenRequestV2TargetWorkspace> workspaceRequests =
        new GenerateTokenRequestV2TargetWorkspace[] {
                new GenerateTokenRequestV2TargetWorkspace(workspaceId)
      };


      GenerateTokenRequestV2 tokenRequest =
        new GenerateTokenRequestV2(datasets: datasetRequests,
                                    reports: reportRequests,
                                    targetWorkspaces: workspaceRequests);


      // call to Power BI Service API and pass GenerateTokenRequest object to generate embed token
      string embedToken = pbiClient.EmbedToken.GenerateToken(tokenRequest).Token;


      viewModel = new {
        workspaces = workspaces,
        currentWorkspace = currentWorkspace.Name,
        datasets = datasets,
        reports = reports,
        token = embedToken
      };

      return JsonConvert.SerializeObject(viewModel);
    }

    public async Task<Group> GetFirstWorkspace() {
      PowerBIClient pbiClient = this.GetPowerBiClient();
      var workspaces = (await pbiClient.Groups.GetGroupsAsync()).Value;
      if (workspaces.Count > 0) {
        return workspaces.First();
      }
      else {
        return null;
      }
    }

  }
}