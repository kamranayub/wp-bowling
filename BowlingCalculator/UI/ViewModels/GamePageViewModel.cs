using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using BowlingCalculator.Core;
using BowlingCalculator.Core.Messages;
using BowlingCalculator.UI.Controls;
using BowlingCalculator.UI.Models;
using BowlingCalculator.UI.Resources;
using BowlingCalculator.UI.Views;
using Caliburn.Micro;
using Microsoft.Phone.Controls;

namespace BowlingCalculator.UI.ViewModels {
    public class GamePageViewModel : BaseViewModel, IHandle<Game.Started>, IHandle<Game.Ended>, IHandle<Game.Reset>, IHandle<Game.InProgress> {
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _events;
        private BowlingPlayer _winner;
        private Bowling _game;

        public GamePageViewModel()
            : this(null, new EventAggregator()) {

            // Mock data for design-time
            if (Execute.InDesignMode) {
                var fake = new FakeGames();

                // modify to change designer reaction
                Game = fake.PerfectGame();
                //Game = fake.GameInProgress();
            }
        }

        public GamePageViewModel(INavigationService navigation, IEventAggregator events) {
            _navigation = navigation;
            _events = events;            

            Game = new Bowling(events);
            RunnerUps = new BindableCollection<BowlingPlayer>();
        }

        public Bowling Game {
            get { return _game; }
            set {
                if (Equals(value, _game)) return;
                _game = value;
                NotifyOfPropertyChange(() => Game);
                NotifyOfPropertyChange(() => CanAddPlayer);
                NotifyOfPropertyChange(() => CanReset);
            }
        }

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
            _events.Subscribe(Game);
        }

        protected override void OnDeactivate(bool close) {
            base.OnDeactivate(close);

            if (close) { // listens in background as singleton
                _events.Unsubscribe(this);
                _events.Unsubscribe(Game);
            }
        }

        public bool CanAddPlayer {
            get { return !Game.IsInProgress; }
        }

        public void AddPlayer() {
            _navigation.UriFor<AddPlayerPageViewModel>().Navigate();
        }

        public bool CanReset {
            get { return Game.IsStarted; }
        }

        public void Reset() {
            var confirmDialog = new CustomMessageBox();

            confirmDialog.Title = AppResources.GamePageResetDialogTitle;
            confirmDialog.Caption = AppResources.GamePageResetDialogContent;

            confirmDialog.IsLeftButtonEnabled = Game.IsInProgress;
            confirmDialog.LeftButtonContent = AppResources.GamePageResetDialogLeftButtonContent;
            confirmDialog.RightButtonContent = AppResources.GamePageResetDialogRightButtonContent;

            confirmDialog.Dismissed += (sender, args) => {
                    if (args.Result == CustomMessageBoxResult.LeftButton) {
                        Game.Reset();
                    } else if (args.Result == CustomMessageBoxResult.RightButton) {
                        Game.Reset(true);
                    }
                };

            confirmDialog.Show();
        }

        public void PickPins(BowlingFrame frame) {
            // Do not show picker if this isn't the current frame
            if (!frame.IsCurrentFrame) {
                return;
            }

            // get available pins for the frame
            var pins = frame.GetAvailablePins();

            // configure pin picker
            var pinPicker = this.DismissableDialogFor<PinPickerViewModel>(
                configureMessageBox: dialog => {
                        dialog.Title = AppResources.PinPickerDialogTitle;
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

        public void About() {
            _navigation.UriFor<AboutPageViewModel>().Navigate();
        }

        #region Message Handling

        public void Handle(Game.Started message) {

            NotifyOfPropertyChange(() => CanReset);
            NotifyOfPropertyChange(() => CanAddPlayer);
        }

        public void Handle(Game.Ended message) {

            NotifyOfPropertyChange(() => CanReset);

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

        public void Handle(Game.Reset message) {

            NotifyOfPropertyChange(() => CanReset);
            NotifyOfPropertyChange(() => CanAddPlayer);

        }

        public void Handle(Game.InProgress message) {

            NotifyOfPropertyChange(() => CanAddPlayer);
            NotifyOfPropertyChange(() => CanReset);

        }

        #endregion
        
    }
}
