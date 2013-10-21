using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using BowlingCalculator.Core.Resources;
using Caliburn.Micro;

namespace BowlingCalculator.Core {
    public class BowlingFrame : PropertyChangedBase {
        private int? _ball1;
        private int? _ball2;
        private int? _ball3;
        private int? _cumulativeScore;
        private bool _isCurrentFrame;

        public int? Ball1 {
            get { return _ball1; }
            private set {
                if (value == _ball1) return;
                _ball1 = value;
                NotifyOfPropertyChange(() => Ball1);
                NotifyOfPropertyChange(() => Ball1DisplayText);
                NotifyOfPropertyChange(() => Ball2DisplayText);
                NotifyOfPropertyChange(() => Ball3DisplayText);
                NotifyOfPropertyChange(() => IsStrike);
                NotifyOfPropertyChange(() => IsSpare);
            }
        }

        public string Ball1DisplayText {
            get {
                if (IsStrike) {
                    if (IsLast) {
                        return CoreResources.StrikeText;
                    }
                    return null;
                } else if (Ball1.HasValue) {
                    return Ball1.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    return null;
                }
            }
        }

        public int? Ball2 {
            get { return _ball2; }
            private set {
                if (value == _ball2) return;
                _ball2 = value;
                NotifyOfPropertyChange(() => Ball2);
                NotifyOfPropertyChange(() => Ball1DisplayText);
                NotifyOfPropertyChange(() => Ball2DisplayText);
                NotifyOfPropertyChange(() => Ball3DisplayText);
                NotifyOfPropertyChange(() => IsStrike);
                NotifyOfPropertyChange(() => IsSpare);
            }
        }

        public string Ball2DisplayText {
            get {
                if (Ball1 == Constants.TotalPins && !IsLast) {
                    return CoreResources.StrikeText;
                } else if (Ball2 == Constants.TotalPins && IsLast) {
                    return CoreResources.StrikeText;
                } else if (IsSpare) {
                    return CoreResources.SpareText;
                } else if (Ball2.HasValue) {
                    return Ball2.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    return null;
                }
            }
        }

        public int? Ball3 {
            get { return _ball3; }
            private set {
                if (value == _ball3) return;
                _ball3 = value;
                NotifyOfPropertyChange(() => Ball3);
                NotifyOfPropertyChange(() => Ball1DisplayText);
                NotifyOfPropertyChange(() => Ball2DisplayText);
                NotifyOfPropertyChange(() => Ball3DisplayText);
                NotifyOfPropertyChange(() => IsStrike);
                NotifyOfPropertyChange(() => IsSpare);
            }
        }

        public string Ball3DisplayText {
            get {
                if (Ball3 == Constants.TotalPins) {
                    return CoreResources.StrikeText;
                } else if (Ball2 + Ball3 == 10) {
                    return CoreResources.SpareText;
                } else if (Ball3.HasValue) {
                    return Ball3.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    return null;
                }
            }
        }

        public int Index { get; set; }

        public int? CumulativeScore {
            get { return _cumulativeScore; }
            set {
                if (value == _cumulativeScore) return;
                _cumulativeScore = value;
                NotifyOfPropertyChange(() => CumulativeScore);
            }
        }

        public bool IsCurrentFrame {
            get { return _isCurrentFrame; }
            set {
                if (value.Equals(_isCurrentFrame)) return;
                _isCurrentFrame = value;
                NotifyOfPropertyChange(() => IsCurrentFrame);
            }
        }

        public bool IsLast {
            get { return (Index + 1) == Constants.Frames; }
        }

        public bool IsStrike {
            get { return Ball1 == Constants.TotalPins || 
                (IsLast && Ball2 == Constants.TotalPins) || 
                (IsLast && Ball3 == Constants.TotalPins); }
        }

        public bool IsSpare {
            get {
                bool isSpare = false;

                if (IsLast && Ball2 != null && Ball3 != null) {
                    isSpare = Ball2.Value + Ball3.Value == Constants.TotalPins;
                }

                if (Ball1 != null && Ball2 != null) {
                    isSpare = Ball1.Value + Ball2.Value == Constants.TotalPins;
                }

                return isSpare;
            }
        }

        public void Bowl(int pins) {
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
                throw new BowlingException(CoreResources.ErrorFrameEnded);
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
                            if (nextFrame.Ball1 != null && nextFrame.Ball2 != null) {
                                return 10 + nextFrame.Ball1.Value + nextFrame.Ball2.Value;
                            }
                        }
                    } else {
                        if (nextFrame.Ball1 == null || nextFrame.Ball2 == null) {
                            return null;
                        } else {
                            return 10 + nextFrame.Ball1.Value + nextFrame.Ball2.Value;
                        }
                    }
                } else {
                    // Last frame and a first or second ball is strike
                    if (Ball2 == Constants.TotalPins && Ball3 != null) {
                        return Ball1 + Ball2 + Ball3;
                    } else if (Ball1 == Constants.TotalPins && Ball2 != null && Ball3 != null) {
                        return Ball1 + Ball2 + Ball3;
                    } else if (IsSpare) {
                        return Ball1 + Ball2 + Ball3;
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
                } else if (nextFrame == null && Ball3 != null) {
                    // last frame
                    return 10 + Ball3.Value;
                }

                // cannot calculate just yet
                return null;
            }

            // Open frame
            if (Ball2 != null && Ball1 != null) {
                return Ball1.Value + Ball2.Value;
            } else {
                return null;
            }
        }

        public bool IsDone() {
            if (IsLast && (IsStrike || IsSpare)) {
                return Ball3 != null;
            }

            return IsStrike || Ball2 != null;
        }

        public int GetAvailablePins() {
            if (Ball1.HasValue && !IsLast) {
                return Constants.TotalPins - Ball1.Value;
            } else if (IsLast) {

                // 10th frame
                if (Ball3 != null) { // frame ended
                    return 0;
                } else if (Ball1 == Constants.TotalPins || Ball2 == Constants.TotalPins || IsSpare) { // second strike or spare

                    // tenth frame when first throw is a strike and second throw isn't a strike
                    if (IsLast && Ball2 != null && Ball2 != Constants.TotalPins && !IsSpare) {
                        return Constants.TotalPins - Ball2.Value;
                    }

                    return Constants.TotalPins;
                } else if (Ball1.HasValue) {
                    return Constants.TotalPins - Ball1.Value;
                }
            }

            return Constants.TotalPins;
        }
        
        public void Reset() {
            Ball1 = null;
            Ball2 = null;
            Ball3 = null;
            CumulativeScore = null;
        }
    }
}