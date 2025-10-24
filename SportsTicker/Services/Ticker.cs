using Microsoft.AspNetCore.SignalR;
using SportsTicker.Entities;
using SportsTicker.Hubs;
using SportsTicker.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SportsTicker.Services
{
    public interface ITicker
    {
        Scores GetAllGames();
        Task Publish();
        Task Start();
    }

    public class Ticker(ITickerDAL tickerDal,
        IGameManager gameManager,
        IHubContext<SportsTickerHub> tickerHubContext) : ITicker
    {
        private readonly ITickerDAL _tickerDal = tickerDal;
        private readonly IHubContext<SportsTickerHub> _tickerHubContext = tickerHubContext;
        private readonly IGameManager _gameManager = gameManager;
        private readonly Scores _scores = new();
        private readonly PeriodicTimer _timer;
        private List<Game> _games = [];
        private static readonly Random _random = new();
        private CancellationTokenSource _cts;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task Start()
        {
            await _semaphore.WaitAsync();

            try
            {
                if (_gameManager.GameClock.InProgress) return;
                _gameManager.GameClock.InProgress = true;
                _gameManager.GameClock.FinalBuzzer = false;
                _games = await _tickerDal.GetGamesAsync();
                _gameManager.GameClock.Clock.Start();
                _cts = new CancellationTokenSource();
                _ = RunAsync(_cts.Token);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            using PeriodicTimer _timer = new(TimeSpan.FromSeconds(_gameManager.GameClock.TickerInterval));

            do
            {
                await Publish();
            }
            while (await _timer.WaitForNextTickAsync(cancellationToken));
        }

        public Scores GetAllGames()
        {
            return new Scores { Games = _games, GameClock = _gameManager.GameClock };
        }

        public async Task Publish()
        {
            _gameManager.Period.ElapsedTime();

            if (!_gameManager.GameClock.Timeout && !_gameManager.GameClock.FinalBuzzer)
            {
                // decide which games have updated information
                foreach (var game in _games)
                {
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

                    if (scored)
                    {
                        // how much did they score
                        var points = _random.Next(2, 4);
                        opponent.PointsPerQuarter[_gameManager.GameClock.Quarter - 1] += points;

                        // add player points
                        opponent.Team.Players[playerIndex].Stats.Points += points;

                        // who got the assist
                        var assisted = BooleanGenerator.NextBoolean();

                        if (assisted)
                        {
                            var playerAssistIndex = RandomGenerator.NotIn(0, numberOfPlayers, playerIndex);
                            opponent.Team.Players[playerAssistIndex].Stats.Assists++;
                        }
                    }
                    else
                    {
                        // add rebounds
                        opponent.Team.Players[playerIndex].Stats.Rebounds++;
                    }
                }
            }

            _scores.Games = _games;
            _scores.GameClock = _gameManager.GameClock;
            await _tickerHubContext.Clients.All.SendCoreAsync("UpdateScores", [_scores]);

            if (_gameManager.GameClock.FinalBuzzer)
            {
                _gameManager.GameClock.InProgress = false;
                _cts.Cancel();
                _timer?.Dispose();
            }
        }

        public class RandomGenerator
        {
            private static readonly Random _random = new();

            public static int NotIn(int min, int max, int not)
            {
                int number = int.MinValue;

                for (int i = 0; i < 10000; i++)
                {
                    number = _random.Next(min, max);
                    if (number != not) break;
                }

                return number;
            }
        }

        public class BooleanGenerator
        {
            private static readonly Random _random = new();

            public static bool NextBoolean()
            {
                return Convert.ToBoolean(_random.Next(0, 2));
            }
        }
    }
}
