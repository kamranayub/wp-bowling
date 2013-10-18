namespace BowlingCalculator.UI.Messages {
    public struct PlayerAddedMessage {
        public string Player;

        public PlayerAddedMessage(string player) {
            Player = player;
        }
    }
}