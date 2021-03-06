﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

namespace BowlingCalculator.Core {
    public class BowlingPlayer : PropertyChangedBase {
        private int _score;

        public BowlingPlayer() {
            Frames = new BindableCollection<BowlingFrame>();
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
                NotifyOfPropertyChange(() => Score);
            }
        }

        public IObservableCollection<BowlingFrame> Frames { get; set; }       

        public void Bowl(int currentFrame, int pins) {
            Frames[currentFrame - 1].Bowl(pins);
        }

        public int GetScore() {
            return Frames.Sum(f => f.GetScore(Frames).GetValueOrDefault());
        }
        
        public void Reset() {
            Score = 0;

            foreach (var frame in Frames) {
                frame.Reset();
            }
        }
    }
}