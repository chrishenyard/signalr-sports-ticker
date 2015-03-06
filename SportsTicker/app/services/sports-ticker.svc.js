'use strict';

(function () {
	var sportsTickerData = function ($rootScope) {
		function sportsTickerOperations() {
			var connection;
			var proxy;
			var updateScores;
			var setValues;

			var setCallbacks = function (setValuesCallback, updateScoresCallback) {
				setValues = setValuesCallback;
				updateScores = updateScoresCallback;
			};

			var initializeClient = function () {
				connection = $.hubConnection();
				proxy = connection.createHubProxy('sportsTicker');
				configureProxyClientFunctions();
				start();
			};

			var configureProxyClientFunctions = function () {
				proxy.on('updateScores', function (scores) {
					$rootScope.$apply(updateScores(scores));
				});
			};

			var initializeScores = function () {
				proxy.invoke('getAllGames').done(function (scores) {
					$rootScope.$apply(setValues(scores));
				}).pipe(function () {
					proxy.invoke('startTicker');
				});
			};

			var start = function () {
				connection.start().pipe(function () {
					initializeScores();
				});
			};

			return {
				initializeClient: initializeClient,
				setCallbacks: setCallbacks
			}
		};

		return sportsTickerOperations;
	};

	var highLight = function () {
		function link(scope, element, attr) {
			var highlightClass = 'highlight';

			scope.$watch(attr.highLight, function (newVal, oldVal) {
				if (newVal !== oldVal) {
					element.clearQueue().queue(function (next) {
						$(this).addClass(highlightClass); next();
					}).delay(1000).queue(function (next) {
						$(this).removeClass(highlightClass); next();
					});
				}
			});
		}

		return {
			restrict: 'A',
			link: link,
			scope: { opponent: '=' }
		};
	};

	angular.module('sportsTickerServices', [])
		.factory('sportsTickerData', ['$rootScope', sportsTickerData])
		.directive('highLight', [highLight]);
})();
