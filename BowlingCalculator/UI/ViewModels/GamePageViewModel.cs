using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BowlingCalculator.Core;
using BowlingCalculator.UI.Models;

namespace BowlingCalculator.UI.ViewModels {
    public class GamePageViewModel : BaseViewModel {
        private BowlingPlayer _winner;

        public GamePageViewModel() {
            Game = new Bowling();
            RunnerUps = new ObservableCollection<BowlingPlayer>();

            // Mock data for design-time
            if (DesignerProperties.IsInDesignTool) {
                var fake = new FakeGames();

                // modify to change designer reaction
                Game = fake.PerfectGame();
                GameOnPropertyChanged(this, new PropertyChangedEventArgs("IsEnded"));
            }

            Game.PropertyChanged += GameOnPropertyChanged;
        }

        private void GameOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsEnded") {
                GameIsEnded();
            }
        }

        private void GameIsEnded() {
            if (Game.IsEnded) {
                
                // order players
                var players = Game.Players.OrderByDescending(p => p.Score);

                // winner
                Winner = players.FirstOrDefault();

                // runner ups
                RunnerUps.Clear();
                
                foreach (var runnerUp in players.Skip(1)) {
                    RunnerUps.Add(runnerUp);
                }
            }
        }

        public Bowling Game { get; set; }

        public BowlingPlayer Winner {
            get { return _winner; }
            set {
                if (Equals(value, _winner)) return;
                _winner = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BowlingPlayer> RunnerUps { get; set; }
    }
}
