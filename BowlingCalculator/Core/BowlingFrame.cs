using System.Collections.Generic;
using System.Linq;

namespace BowlingCalculator.Core {
    public class BowlingFrame {
        public int? Ball1 { get; private set; }
        public int? Ball2 { get; private set; }
        public int? Ball3 { get; private set; } // 10th frame only

        public void Bowl(int pins, IList<BowlingFrame> frames) {
            if (Ball1 == null) {
                Ball1 = pins;
                return;
            }

            if (Ball2 == null) {
                Ball2 = pins;
                return;
            }

            if (IsLast && Ball3 == null) {
                Ball3 = pins;
                return;
            }

            if (Ball1 != null && Ball2 != null) {
                throw new BowlingException("Frame is already over");
            }
        }

        public int? GetScore(IList<BowlingFrame> frames) {
            
            var nextFrame = frames.ElementAtOrDefault(frames.IndexOf(this) + 1);

            // Strike
            if (IsStrike) {

                if (nextFrame != null) {
                    // Is next ball a strike? If so, we'll need the next frame
                    if (nextFrame.IsStrike) {
                        var thirdFrame = frames.ElementAtOrDefault(frames.IndexOf(nextFrame) + 1);

                        // Get first throw of third frame, if it's a strike it's just +10
                        if (thirdFrame != null && thirdFrame.Ball1 != null) {
                            return 20 + thirdFrame.Ball1.Value;
                        } else if (thirdFrame == null && nextFrame.IsLast) {
                            // 10th frame
                            if (nextFrame.Ball2 != null) {
                                return 20 + nextFrame.Ball2.Value;
                            }
                        }
                    }
                    else {

                        int? nextScore = nextFrame.GetScore(frames);

                        if (nextScore == null) {
                            return null;
                        }
                        else {
                            return 10 + nextScore.Value;
                        }
                    }
                }
                else {
                    // Last frame and a first or second ball is a strike
                    if (Ball2 == 10 && Ball3 != null) {
                        return 20 + Ball3;
                    } else if (Ball1 == 10 && Ball2 != null && Ball3 != null) {
                        return 10 + Ball2 + Ball3;
                    }
                }

                // cannot calculate just yet
                return null;
            }

            // Spare
            if (IsSpare) {

                // Get next ball
                if (nextFrame != null && nextFrame.Ball1 != null) {
                    return 10 + nextFrame.Ball1.Value;
                }

                // cannot calculate just yet
                return null;
            }

            // Open frame
            if (Ball2 != null && Ball1 != null) {
                return Ball1.Value + Ball2.Value;
            }
            else {
                return null;
            }
        }

        public bool IsDone() {
            if (IsLast) {
                return Ball3 != null;
            }

            return IsStrike || Ball2 != null;
        }

        public bool IsLast { get; set; }

        public bool IsStrike {
            get { return Ball1 == 10 || (IsLast && Ball2 == 10); }
        }

        public bool IsSpare {
            get {
                if (Ball1 != null && Ball2 != null) {
                    return Ball1.Value + Ball2.Value == 10;
                }

                return false;
            }
        }
    }
}