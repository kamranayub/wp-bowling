using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using BowlingCalculator.Core;
using BowlingCalculator.UI.Controls;
using BowlingCalculator.UI.Messages;
using BowlingCalculator.UI.Models;
using BowlingCalculator.UI.Views;
using Caliburn.Micro;
using Microsoft.Phone.Controls;

namespace BowlingCalculator.UI.ViewModels {
    public class GamePageViewModel : BaseViewModel, IHandle<PlayerAddedMessage> {
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _events;
        private readonly Func<PinPickerViewModel> _pinPickerFactory;
        private BowlingPlayer _winner;

        public GamePageViewModel()
            : this(null, null, null) {

            // Mock data for design-time
            if (Execute.InDesignMode) {
                var fake = new FakeGames();

                // modify to change designer reaction
                //Game = fake.PerfectGame();
                //Game = fake.GameInProgress();
                GameOnPropertyChanged(this, new PropertyChangedEventArgs("IsEnded"));
            }
        }

        public GamePageViewModel(INavigationService navigation, IEventAggregator events, Func<PinPickerViewModel> pinPickerFactory) {
            _navigation = navigation;
            _events = events;
            _pinPickerFactory = pinPickerFactory;

            Game = new Bowling();
            RunnerUps = new BindableCollection<BowlingPlayer>();

            Game.PropertyChanged += GameOnPropertyChanged;
        }

        public Bowling Game { get; set; }

        public BowlingPlayer Winner {
            get { return _winner; }
            set {
                if (Equals(value, _winner)) return;
                _winner = value;
                NotifyOfPropertyChange(() => Winner);
            }
        }

        public IObservableCollection<BowlingPlayer> RunnerUps { get; set; }

        protected override void OnActivate() {
            base.OnActivate();
            _events.Subscribe(this);
        }

        protected override void OnDeactivate(bool close) {
            base.OnDeactivate(close);

            if (close) { // listens in background as singleton
                _events.Unsubscribe(this);
            }
        }

        public void About() {
            _navigation.UriFor<AboutPageViewModel>().Navigate();
        }

        public bool CanAddPlayer {
            get { return !Game.IsStarted; }
        }

        public void AddPlayer() {
            _navigation.UriFor<AddPlayerPageViewModel>().Navigate();
        }

        public bool CanReset {
            get { return Game.IsStarted; }
        }

        public void Reset() {
            var confirmDialog = new CustomMessageBox();

            confirmDialog.Title = "Reset the game";
            confirmDialog.Caption = "Everyone's score will be reset and you'll start over, is that OK?";
            confirmDialog.LeftButtonContent = "Yes";
            confirmDialog.RightButtonContent = "No";

            confirmDialog.Dismissed += (sender, args) => {
                    if (args.Result == CustomMessageBoxResult.LeftButton) {
                        Game.Reset();
                    }
                };

            confirmDialog.Show();
        }

        public void PickPins(BowlingFrame frame) {
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

            // configure pin picker
            var pinPicker = this.DismissableDialogFor<PinPickerViewModel>(
                configureMessageBox: dialog => {
                        dialog.Title = "Pins Knocked Down";
                    },
                configureViewModel: pinPickerModel => {
                        pinPickerModel.AvailablePins = pins;
                    },
                onDismissed: (pinPickerModel, dismissedArgs) => {
                        if (pinPickerModel.SelectedPins != null) {
                            Game.Bowl(pinPickerModel.SelectedPins.Value);
                        }
                    }
            );

            pinPicker.Show();
        }

        /// <summary>
        /// Handles global event PlayerAddedMessage and adds player
        /// </summary>
        /// <param name="message"></param>
        public void Handle(PlayerAddedMessage message) {
            Game.AddPlayer(message.Player);
        }

        private void GameOnPropertyChanged(object sender, PropertyChangedEventArgs e) {

            // hokey!
            // maybe better to use event handlers
            // or just bite the bullet and use IEventAggregator!

            if (e.PropertyName == "IsEnded") {
                OnGameIsEndedChanged();
            } else if (e.PropertyName == "IsStarted") {
                OnGameStartedChanged();
            }
        }

        private void OnGameStartedChanged() {
            NotifyOfPropertyChange(() => CanAddPlayer);
            NotifyOfPropertyChange(() => CanReset);
        }

        private void OnGameIsEndedChanged() {
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
