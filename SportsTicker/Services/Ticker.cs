using System;
using System.Collections.Generic;
using System.Threading;
using SportsTicker.Entities;
using SportsTicker.Repositories;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.FileSystems;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace SportsTicker.Services {

	public class Ticker {
		private readonly static Lazy<Ticker> _instance = new Lazy<Ticker>(
			() => new Ticker(
				new TickerDAL(
					new TickerFileRepository(),
					new PhysicalFileSystem(@".\").Root),
				GlobalHost.ConnectionManager.GetHubContext<SportsTickerHub>().Clients)
		);

		private ITickerDAL _tickerDal;
		private IHubConnectionContext<dynamic> _clients;
		private Timer _timer;
		private List<Game> _games;
		private GameManager _gameManager = new GameManager();
		private Scores _scores = new Scores();
		private static Random _random = new Random();

		private Ticker(ITickerDAL tickerDal, IHubConnectionContext<dynamic> clients) {
			_tickerDal = tickerDal;
			_clients = clients;
		}

		public static Ticker Instance {
			get {
				return _instance.Value;
			}
		}

		public async Task Start() {
			if (_gameManager.GameClock.InProgress) return; 
			_gameManager.GameClock.InProgress = true;
			await _tickerDal.ResetGamesAsync();
			_games = await _tickerDal.GetGamesAsync();
			_gameManager.GameClock.Clock.Start();
			_timer = new Timer(Publish, null, 0, System.Threading.Timeout.Infinite);
		}

		public void Stop() {
			_timer.Dispose();
		}

		public async Task<Scores> GetAllGames() {
			var games = await _tickerDal.GetGamesAsync();
			return new Scores { Games = games, GameClock = _gameManager.GameClock };
		}

		public void Publish(object state) {
			var games = new List<Game>();

			_gameManager.Period.ElapsedTime();

			if (!_gameManager.GameClock.Timeout) {
				// decide which games have updated information
				foreach (var game in _games) {
					var updateGame = BooleanGenerator.NextBoolean();
					if (!updateGame) continue;

					// anyone score ?
					var scored = BooleanGenerator.NextBoolean();

					// which team
					var opponentIndex = _random.Next(0, 2);
					var opponent = game.Opponents[opponentIndex];

					// which player	
					var numberOfPlayers = opponent.Team.Players.Count;
					var playerIndex = _random.Next(0, numberOfPlayers);

					if (scored) {
						// how much did they score
						var points = _random.Next(2, 4);
						opponent.PointsPerQuarter[_gameManager.GameClock.Quarter - 1] += points;

						// add player points
						opponent.Team.Players[playerIndex].Stats.Points += points;

						// who got the assist
						var assisted = BooleanGenerator.NextBoolean();

						if (assisted) {
							var playerAssistIndex = RandomGenerator.NotIn(0, numberOfPlayers, playerIndex);
							opponent.Team.Players[playerAssistIndex].Stats.Assists++;
						}
					}
					else {
						// add rebounds
						opponent.Team.Players[playerIndex].Stats.Rebounds++;
					}

					games.Add(game);
				}
			}

			_scores.Games = games;
			_scores.GameClock = _gameManager.GameClock;

			UpdateScores(_scores);

			if (!_gameManager.GameClock.FinalBuzzer) {
				_timer.Change(_gameManager.GameClock.TickerInterval, System.Threading.Timeout.Infinite);
			}
			else {
				_gameManager.GameClock.InProgress = false;
			}
		}

		public void UpdateScores(Scores _scores) {
			_clients.All.updateScores(_scores);
		}

		public class RandomGenerator {
			private static Random _random = new Random();

			public static int NotIn(int min, int max, int not) {
				int number = int.MinValue;

				for (int i = 0; i < 10000; i++) {
					number = _random.Next(min, max);
					if (number != not) break;
				}

				return number;
			}
		}

		public class BooleanGenerator {
			private static Random _random = new Random();

			public static bool NextBoolean() {
				return Convert.ToBoolean(_random.Next(0, 2));
			}
		}
	}
}
