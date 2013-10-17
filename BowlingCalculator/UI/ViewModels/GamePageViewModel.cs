using System.Collections.Generic;
using System.ComponentModel;
using BowlingCalculator.Core;

namespace BowlingCalculator.UI.ViewModels {
    public class GamePageViewModel {

        public GamePageViewModel() {
            Game = new Bowling();

            // Mock data for design-time
            if (DesignerProperties.IsInDesignTool) {
                Game.AddPlayer("Kamran");
                Game.AddPlayer("Cassie");

                for (var i = 0; i < 2*12; i++) {
                    Game.Bowl(10);
                }
            }
        }

        public Bowling Game { get; set; }
    }
}
