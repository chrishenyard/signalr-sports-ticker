'use strict';

(function () {
	var sportsTickerCtrl = function ($scope, sportsTickerData) {
		var ctrl = this;
		ctrl.games = [];
		ctrl.gameCock = {};
		ctrl.initialized = false;

		function assignGames(scores) {
			ctrl.games = scores.Games;
			ctrl.gameClock = scores.GameClock;
			ctrl.initialized = true;
		}

		function updateScores(scores) {
			ctrl.gameClock = scores.GameClock;
			for (var index = 0; index < scores.Games.length; index++) {
				var updatedGame = scores.Games[index];
				for (var count = 0; count < ctrl.games.length; count++) {
					var game = ctrl.games[count];
					if (game.Id === updatedGame.Id) {
						$.extend(true, ctrl.games[count], updatedGame);
					}
				}
			}
		}

		var ops = sportsTickerData();
		ops.setCallbacks(assignGames, updateScores);
		ops.initializeClient();
	}

	angular.module('sportsTickerApp', ['sportsTickerServices'])
		.controller('sportsTickerCtrl', ['$scope', 'sportsTickerData', sportsTickerCtrl]);
})();
