import * as $ from 'jquery';

import * as powerbi from "powerbi-client";
import * as pbimodels from "powerbi-models";

require('powerbi-models');
require('powerbi-client');

class Workspace {
  id: string;
  name: string;
  isReadOnly: boolean;
}

class Report {
  id: string;
  name: string;
  datasetId: string;
  embedUrl: string;
  webUrl: string;
}

class Dataset {
  id: string;
  name: string;
}

class ViewModel {
  currentWorkspace: string;
  currentWorkspaceIsReadOnly: boolean;
  workspaces: Workspace[];
  reports: Report[];
  datasets: Dataset[];
  token: string;
}

$(() => {

  $("#embed-toolbar").hide();

  var workspaceSelector: JQuery = $("#workspace-selector");
  var workspacesList: JQuery = $("#workspaces-list");
  var reportsList: JQuery = $("#reports-list");
  var datasetsList: JQuery = $("#datasets-list");

  var viewModel: ViewModel = window['viewModel'] as ViewModel;

  console.log("cw", viewModel.currentWorkspace);

  workspaceSelector.text(viewModel.currentWorkspace);

  var linkMyWorkspace = $("<a>", { "href": "/home/embed/" })
    .text("My Workspace")
    .addClass("dropdown-item");
  workspacesList.append(linkMyWorkspace);

  workspacesList.append($("<div>").addClass("dropdown-divider"))

  viewModel.workspaces.forEach((workspace: Workspace) => {
    var link = $("<a>", { "href": "/home/embed/?workspaceId=" + workspace.id })
      .text(workspace.name)
      .addClass("dropdown-item");
    workspacesList.append(link);
  });

  if (viewModel.reports.length == 0) {
    reportsList.append($("<li>").text("You have no reports"));
  }
  else {
    viewModel.reports.forEach((report: Report) => {
      var li = $("<li>");
      li.append($("<i>").addClass("fa fa-bar-chart"));
      li.append($("<a>", {
        "href": "javascript:void(0);"
      }).text(report.name).click(() => { embedReport(report) }));
      reportsList.append(li);
    });
  }

  if (viewModel.datasets.length == 0) {
    datasetsList.append($("<li>").text("You have no datasets"));
  }
  else {
    viewModel.datasets.forEach((dataset: Dataset) => {
      var li = $("<li>");
      li.append($("<i>").addClass("fa fa-database"));
      li.append($("<a>", {
        "href": "javascript:void(0);"
      }).text(dataset.name).click(() => { embedNewReport(dataset) }));
      datasetsList.append(li);
    });
  }

  // load new report in edit mode as Save action on new report
  var urlParams = new URLSearchParams(window.location.search);
  var newReportId = urlParams.get("newReport");
  if (newReportId) {
    var newReport = viewModel.reports.find(report => report.id == newReportId);
    embedReport(newReport, true);
  }

});


var embedReport = (report: Report, editMode: boolean = false) => {
  $("#embedding-instructions").hide();

  var viewModel: ViewModel = window['viewModel'];
  var token: string = viewModel.token;

  var models = pbimodels;

  var config: powerbi.IEmbedConfiguration = {
    type: 'report',
    id: report.id,
    embedUrl: report.embedUrl,
    accessToken: token,
    tokenType: models.TokenType.Aad,
    permissions: models.Permissions.All,    
    viewMode: editMode ? models.ViewMode.Edit : models.ViewMode.View,
    settings: {
      useCustomSaveAsDialog: true,
      panes: {
        filters: { visible: false },
        pageNavigation: { visible: true }
      }
    }
  };

  // Get a reference to the embedded report HTML element
  var reportContainer = document.getElementById('embed-container')!;

  var powerbi: powerbi.service.Service = window.powerbi;

  // Embed the report and display it within the div container.
  powerbi.reset(reportContainer);
  var embeddedReport: powerbi.Report = <powerbi.Report>powerbi.embed(reportContainer, config);

  embeddedReport.on("saved", function (event: any) {
    // check for saveAs
    if (event.detail.saveAs) {
      if (window.location.href.includes("?")) {
        window.location.href = window.location.href + "&newReport=" + event.detail.reportObjectId;
      }
      else {
        window.location.href = window.location.href + "?newReport=" + event.detail.reportObjectId;
      }
    }

  });

  var viewMode = editMode ? "edit" : "view";

  $("#breadcrumb").text("Reports > " + report.name);
  $("#embed-toolbar").show();

  if (viewModel.currentWorkspaceIsReadOnly) {
    $("#toggle-edit").hide();
  }
  else {
    $("#toggle-edit").show();
    $("#toggle-edit").unbind("click");
    $("#toggle-edit").click(function () {
      // toggle between view and edit mode
      viewMode = (viewMode == "view") ? "edit" : "view";
      embeddedReport.switchMode(viewMode);
      // show filter pane when entering edit mode
      var showFilterPane = (viewMode == "edit");
      embeddedReport.updateSettings({
        panes: {
          filters: { visible: showFilterPane, expanded: false }
        }
      });
    });

    $("#full-screen").unbind("click");
    $("#full-screen").click(function () {
      embeddedReport.fullscreen();
    });

  };

}

var embedNewReport = (dataset: Dataset) => {
  $("#embedding-instructions").hide();

  var viewModel: ViewModel = window['viewModel'];
  var token: string = viewModel.token

  var models = pbimodels;

  var config: powerbi.IEmbedConfiguration = {
    datasetId: dataset.id,
    embedUrl: "https://app.powerbi.com/reportEmbed",
    accessToken: token,
    tokenType: models.TokenType.Aad,
    settings: {
      panes: {
        filters: { visible: true, expanded: false }
      }
    }
  };

  // Get a reference to the embedded report HTML element
  var reportContainer = document.getElementById('embed-container');

  var powerbi: powerbi.service.Service = window.powerbi;

  // Embed the report and display it within the div container.
  powerbi.reset(reportContainer);
  var embeddedReport = powerbi.createReport(reportContainer, config);

  $("#breadcrumb").text("Datasets > " + dataset.name + " > New Report");
  $("#embed-toolbar").show();

  $("#toggle-edit").hide();
  $("#full-screen").unbind("click");
  $("#full-screen").click(function () {
    embeddedReport.fullscreen();
  });

  // handle save action on new report to refresh page
  embeddedReport.on("saved", function (event: any) {
    console.log(event);
    console.log((typeof event));
    if (window.location.href.includes("?")) {
      window.location.href = window.location.href + "&newReport=" + event.detail.reportObjectId;
    }
    else {
      window.location.href = window.location.href + "?newReport=" + event.detail.reportObjectId;
    }

  });

  embeddedReport.on("saved", function (event: any) {
    console.log(event);
    console.log((typeof event));
    if (window.location.href.includes("?")) {
      window.location.href = window.location.href + "&newReport=" + event.detail.reportObjectId;
    }
    else {
      window.location.href = window.location.href + "?newReport=" + event.detail.reportObjectId;
    }

  });

};


// reset left-nav to height of browser window
$(function () {
  var heightBuffer = 8;
  var newHeight = window.innerHeight - ($("#banner").height() + heightBuffer);
  $("#left-nav").height(newHeight);
  $("#content-box").height(newHeight);
  $(window).resize(function () {
    var newHeight = window.innerHeight - ($("#banner").height() + heightBuffer);
    $("#left-nav").height(newHeight);
    $("#content-box").height(newHeight);
  });
});
