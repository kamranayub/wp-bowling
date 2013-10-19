namespace BowlingCalculator.Core.Messages {
    public struct RequestAddPlayerMessage {
        public string Player;

        public RequestAddPlayerMessage(string player) {
            Player = player;
        }
    }
}