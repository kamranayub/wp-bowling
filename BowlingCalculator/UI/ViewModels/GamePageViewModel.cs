using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BowlingCalculator.Core;
using BowlingCalculator.UI.Models;
using BowlingCalculator.UI.Views;
using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class GamePageViewModel : BaseViewModel {
        private BowlingPlayer _winner;

        public GamePageViewModel() {
            Game = new Bowling();
            RunnerUps = new ObservableCollection<BowlingPlayer>();

            // Mock data for design-time
            if (Execute.InDesignMode) {
                var fake = new FakeGames();

                // modify to change designer reaction
                Game = fake.PerfectGame();
                //Game = fake.GameInProgress();
                GameOnPropertyChanged(this, new PropertyChangedEventArgs("IsEnded"));
            }

            Game.PropertyChanged += GameOnPropertyChanged;
        }

        public string Player1 { get; set; }

        public string Player2 { get; set; }

        public Bowling Game { get; set; }

        public BowlingPlayer Winner {
            get { return _winner; }
            set {
                if (Equals(value, _winner)) return;
                _winner = value;
                NotifyOfPropertyChange(() => Winner);
            }
        }

        public ObservableCollection<BowlingPlayer> RunnerUps { get; set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            if (!String.IsNullOrWhiteSpace(Player1))
                Game.AddPlayer(Player1);

            if (!String.IsNullOrWhiteSpace(Player2))
                Game.AddPlayer(Player2);
        }

        public void PickPins(BowlingFrame frame) {

            if (frame != null) {
                
                // Do not show picker if this isn't the current frame
                if (!frame.IsCurrentFrame) {
                    return;
                }

                // Do not show picker if the frame has been completed
                if (frame.IsDone()) {
                    MessageBox.Show("Sorry, the frame has ended!");
                    return;
                }

                // get available pins for the frame
                var pins = frame.GetAvailablePins();

                // get view (View Aware)
                // TODO: Refactor to avoid this!
                var gamePageView = GetView() as GamePage;

                if (gamePageView != null) {
                    gamePageView.PinPicker.Items.Clear();
                    for (var i = 0; i <= pins; i++) {
                        gamePageView.PinPicker.Items.Add(i);
                    }

                    gamePageView.PinPicker.Open();

                    RoutedEventHandler selectionChanged = null;
                    selectionChanged = (s, e2) =>
                        {
                            gamePageView.PinPicker.Loaded -= selectionChanged;

                            Game.Bowl((int)gamePageView.PinPicker.SelectedItem);
                        };
                    gamePageView.PinPicker.Loaded += selectionChanged;
                }
            }
        }

        private void GameOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsEnded") {
                OnGameIsEnded();
            }
        }

        private void OnGameIsEnded() {
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

                
    }
}
