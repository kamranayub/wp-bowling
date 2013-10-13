using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BowlingCalculator.Core {
    public class Bowling {
        public Bowling() {
            Players = new ObservableCollection<BowlingPlayer>();
        }

        public int CurrentFrame { get; private set; }

        public BowlingPlayer CurrentPlayer { get; private set; }

        public IList<BowlingPlayer> Players { get; private set; }

        public bool IsEnded { get; set; }

        public void Start() {
            if (Players.Count == 0) {
                throw new BowlingException("There must be at least one player");
            }

            CurrentFrame = 1;
            CurrentPlayer = Players[0];
        }

        public BowlingPlayer AddPlayer(string playerName) {
            var newPlayer = new BowlingPlayer() {Name = playerName};

            Players.Add(newPlayer);

            return newPlayer;
        }

        public void Bowl(int pins) {
            if (CurrentPlayer == null) return;

            CurrentPlayer.Bowl(CurrentFrame, pins);

            bool isEndOfTurn = CurrentPlayer.Frames[CurrentFrame - 1].IsDone();
            bool isLastPlayer = Players.Last() == CurrentPlayer;

            if (CurrentFrame == Constants.Frames && isLastPlayer) {
                // End game
                IsEnded = true;
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
        }
    }

    public class BowlingException : Exception {
        public BowlingException(string message) : base(message) {

        }
    }
}
