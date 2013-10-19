using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BowlingCalculator.Core.Annotations;

namespace BowlingCalculator.Core {
    public class Bowling : INotifyPropertyChanged {
        private bool _isEnded;
        private int _currentFrame;
        private BowlingPlayer _currentPlayer;
        private bool _isStarted;

        public Bowling() {
            Players = new ObservableCollection<BowlingPlayer>();

            CurrentFrame = 1;
        }

        public int CurrentFrame {
            get { return _currentFrame; }
            private set {
                if (value == _currentFrame) return;
                _currentFrame = value;
                OnPropertyChanged();
            }
        }

        public BowlingPlayer CurrentPlayer {
            get { return _currentPlayer; }
            private set {
                if (Equals(value, _currentPlayer)) return;
                _currentPlayer = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BowlingPlayer> Players { get; private set; }

        public bool IsEnded {
            get { return _isEnded; }
            set {
                if (value.Equals(_isEnded)) return;
                _isEnded = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// At least one player has bowled
        /// </summary>
        public bool IsStarted {
            get { return _isStarted; }
            set {
                if (value.Equals(_isStarted)) return;
                _isStarted = value;
                OnPropertyChanged();
            }
        }

        public BowlingPlayer AddPlayer(string playerName) {
            var newPlayer = new BowlingPlayer() {Name = playerName};

            Players.Add(newPlayer);            
            OnPropertyChanged("Players");

            // game hasn't started
            if (CurrentPlayer == null) {
                CurrentPlayer = Players[0];
                CurrentPlayer.Frames.First().IsCurrentFrame = true;
            }

            return newPlayer;
        }

        public void Bowl(int pins) {
            if (CurrentPlayer == null || IsEnded) return;

            // Started
            if (!IsStarted) {
                IsStarted = true;
            }

            // Take turn
            CurrentPlayer.Bowl(CurrentFrame, pins);

            // Update frame scores
            int? prevScore = 0;
            int prevCumulativeScore = 0;
            for (var i = 0; i < CurrentPlayer.Frames.Count; i++) {
                var frame = CurrentPlayer.Frames[i];
                int? score = frame.GetScore(CurrentPlayer.Frames);                

                if (frame.IsDone() && prevScore != null && score != null) {
                    frame.CumulativeScore = prevCumulativeScore + score;
                }

                prevScore = score;
                prevCumulativeScore += score.GetValueOrDefault();
            }

            // Update player scores
            CurrentPlayer.Score = CurrentPlayer.GetScore();

            bool isEndOfTurn = CurrentPlayer.Frames[CurrentFrame - 1].IsDone();
            bool isLastPlayer = Players.Last() == CurrentPlayer;

            if (CurrentFrame == Constants.Frames && isLastPlayer && isEndOfTurn) {
                // End game
                EndGame();
                return;
            } else if (isEndOfTurn && isLastPlayer) {
                // Advance frame
                NextFrame();
            }
            
            if (isEndOfTurn) {
                // Advance turn
                NextTurn();
            }
        }

        public void Reset() {
            IsStarted = false;
            IsEnded = false;
            CurrentPlayer = Players[0];
            CurrentFrame = 1;

            foreach (var player in Players) {
                player.Reset();
            }

            UpdateCurrentFrame();
        }

        private void NextFrame() {
            CurrentFrame++;            
        }

        private void NextTurn() {
            var curIndex = Players.IndexOf(CurrentPlayer);

            if (Players.ElementAtOrDefault(curIndex + 1) == null) {
                CurrentPlayer = Players[0];
            }
            else {
                CurrentPlayer = Players[curIndex + 1];
            }

            UpdateCurrentFrame();
        }

        private void EndGame() {
            IsEnded = true;
            UpdateCurrentFrame();
        }

        private void UpdateCurrentFrame() {            
            foreach (var player in Players) {
                foreach (var frame in player.Frames) {
                    frame.IsCurrentFrame = !IsEnded &&
                        CurrentFrame == frame.Index + 1 &&
                        CurrentPlayer == player;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
