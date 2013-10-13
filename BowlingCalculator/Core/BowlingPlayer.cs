using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BowlingCalculator.Core {
    public class BowlingPlayer {
        public BowlingPlayer() {
            Frames = new ObservableCollection<BowlingFrame>();
            for (var i = 0; i < Constants.Frames; i++) {
                var frame = new BowlingFrame();
                Frames.Add(frame);

                if ((i + 1) == Constants.Frames) {
                    frame.IsLast = true;
                }
            }
        }

        public string Name { get; set; }        

        public IList<BowlingFrame> Frames { get; set; }       

        public void Bowl(int currentFrame, int pins) {
            Frames[currentFrame - 1].Bowl(pins, Frames);
        }

        public int GetScore() {
            return Frames.Sum(f => f.GetScore(Frames).GetValueOrDefault());
        }
    }
}