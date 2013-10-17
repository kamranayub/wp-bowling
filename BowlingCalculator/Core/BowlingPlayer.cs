using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BowlingCalculator.Core.Annotations;

namespace BowlingCalculator.Core {
    public class BowlingPlayer : INotifyPropertyChanged {
        private int _score;

        public BowlingPlayer() {
            Frames = new ObservableCollection<BowlingFrame>();
            for (var i = 0; i < Constants.Frames; i++) {
                var frame = new BowlingFrame() { Index = i };
                Frames.Add(frame);
            }
        }

        public string Name { get; set; }

        public int Score {
            get { return _score; }
            set {
                if (value == _score) return;
                _score = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BowlingFrame> Frames { get; set; }       

        public void Bowl(int currentFrame, int pins) {
            Frames[currentFrame - 1].Bowl(pins);
        }

        public int GetScore() {
            return Frames.Sum(f => f.GetScore(Frames).GetValueOrDefault());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset() {
            Score = 0;

            foreach (var frame in Frames) {
                frame.Reset();
            }
        }
    }
}