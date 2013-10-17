using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BowlingCalculator.Core;

namespace BowlingCalculator.UI.Models {
    public class FakeGames {

        private readonly Bowling _game;

        public FakeGames() {
            _game = new Bowling();
            _game.AddPlayer("Kamran");
            _game.AddPlayer("Cassie");
        }

        public Bowling PerfectGame() {
            
            for (var i = 0; i < 12*2; i++) {
                _game.Bowl(10);
            }

            return _game;
        }

        public Bowling GameInProgress() {
            _game.Bowl(10);
            _game.Bowl(0);
            _game.Bowl(10);
            _game.Bowl(0);
            _game.Bowl(0);
            _game.Bowl(9);
            _game.Bowl(1);
            _game.Bowl(2);
            _game.Bowl(3);

            return _game;
        }

    }
}
