using System;
using System.Linq;
using BowlingCalculator.Core.Messages;
using Caliburn.Micro;

namespace BowlingCalculator.Core {
    public class Bowling : PropertyChangedBase, IHandle<RequestAddPlayerMessage> {
        private readonly IEventAggregator _events;
        private bool _isEnded;
        private int _currentFrame;
        private BowlingPlayer _currentPlayer;
        private bool _isInProgress;
        
        public Bowling(IEventAggregator events) {
            if (events == null) throw new ArgumentNullException("events");
            _events = events;

            Players = new BindableCollection<BowlingPlayer>();
            CurrentFrame = 1;
        }

        /// <summary>
        /// The players involved in the game
        /// </summary>
        public IObservableCollection<BowlingPlayer> Players { get; private set; }

        /// <summary>
        /// The current frame
        /// </summary>
        public int CurrentFrame {
            get { return _currentFrame; }
            private set {
                if (value == _currentFrame) return;
                _currentFrame = value;
                NotifyOfPropertyChange(() => CurrentFrame);
            }
        }

        /// <summary>
        /// The current player
        /// </summary>
        public BowlingPlayer CurrentPlayer {
            get { return _currentPlayer; }
            private set {
                if (Equals(value, _currentPlayer)) return;
                _currentPlayer = value;
                NotifyOfPropertyChange(() => CurrentPlayer);
                NotifyOfPropertyChange(() => IsStarted);
            }
        }        

        /// <summary>
        /// Whether or not the game has ended (turns finished)
        /// </summary>
        public bool IsEnded {
            get { return _isEnded; }
            set {
                if (value.Equals(_isEnded)) return;
                _isEnded = value;
                NotifyOfPropertyChange(() => IsEnded);
                NotifyOfPropertyChange(() => IsStarted);
            }
        }

        /// <summary>
        /// Whether or not the game is in progress; at least one player has bowled
        /// </summary>
        public bool IsInProgress {
            get { return _isInProgress; }
            set {
                if (value.Equals(_isInProgress)) return;
                _isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        /// <summary>
        /// Whether or the game has started; at least one player needs to be present or the game can be over
        /// </summary>
        public bool IsStarted {
            get { return CurrentPlayer != null || IsEnded; }
        }

        /// <summary>
        /// Add a new player to the game
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public BowlingPlayer AddPlayer(string playerName) {
            var newPlayer = new BowlingPlayer() {Name = playerName};

            // add player to game
            Players.Add(newPlayer);

            // game hasn't started
            if (!IsStarted) {
                CurrentPlayer = Players[0];
                CurrentPlayer.Frames.First().IsCurrentFrame = true;

                _events.Publish(new Game.Started());
            }

            _events.Publish(new Game.PlayerAdded());

            return newPlayer;
        }

        /// <summary>
        /// Throw a ball. State machine.
        /// </summary>
        /// <param name="pins"></param>
        public void Bowl(int pins) {
            if (CurrentPlayer == null || IsEnded) return;

            // Started
            if (!IsInProgress) {
                IsInProgress = true;

                _events.Publish(new Game.InProgress());
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
                GameOver();
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

        /// <summary>
        /// Resets the game to a started or not started state
        /// </summary>
        /// <param name="clearPlayers">Whether or not to remove all players</param>
        public void Reset(bool clearPlayers = false) {
            IsInProgress = false;
            IsEnded = false;
            
            CurrentFrame = 1;

            if (clearPlayers) {
                CurrentPlayer = null;
                Players.Clear();
            }
            else {
                CurrentPlayer = Players[0];

                foreach (var player in Players) {
                    player.Reset();
                }

                UpdateCurrentFrame();
            }    
            
            _events.Publish(new Game.Reset());
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

        private void GameOver() {
            IsEnded = true;
            UpdateCurrentFrame();

            _events.Publish(new Game.Ended());
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

        #region Message Handling

        public void Handle(RequestAddPlayerMessage message) {
            if (message.Player != null) {
                AddPlayer(message.Player);
            }
        }

        #endregion
    }
}
