import * as $ from 'jquery';

import * as powerbi from "powerbi-client";
import * as models from "powerbi-models";

// ensure Power BI JavaScript API has loaded
require('powerbi-models');
require('powerbi-client');

class viewModel {
	reportId: string;
	embedUrl: string;
	token: string;
}

$(() => {

	// get DOM object div for report container
	var reportContainer: HTMLElement = document.getElementById("embed-container");

	var viewModel: viewModel = window["viewModel"];

	var config: powerbi.IEmbedConfiguration = {
		type: 'report',
		id: viewModel.reportId,
		embedUrl: viewModel.embedUrl,
		accessToken: viewModel.token,
		permissions: models.Permissions.All,
		tokenType: models.TokenType.Aad,
		viewMode: models.ViewMode.View,
		settings: {
			panes: {
				filters: { expanded: false, visible: true },
				pageNavigation: { visible: true }
			},
			persistentFiltersEnabled: true
		}
	};

	// Embed the report and display it within the div container.
	var report = window.powerbi.embed(reportContainer, config);

	// display report properties in browser console
	console.log(report);	

	// add logic to resize embed container element on window rersize event
	var heightBuffer = 12;
	var newHeight = $(window).height() - ($("header").height() + heightBuffer);
	$("#embed-container").height(newHeight);
	$(window).resize(function () {
		var newHeight = $(window).height() - ($("header").height() + heightBuffer);
		$("#embed-container").height(newHeight);
	});

});