using System;

namespace BowlingCalculator.Core {
    public class BowlingException : Exception {
        public BowlingException(string message) : base(message) {

        }
    }
}