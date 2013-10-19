using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BowlingCalculator.Core.Tests {
    [TestClass]
    public class BowlingTests {

        private Bowling _sut;

        [TestInitialize]
        public void Setup() {
            _sut = new Bowling(new EventAggregator());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Addition_Of_Players() {

            var player1 = _sut.AddPlayer("Kamran");
            var player2 = _sut.AddPlayer("Cassie");

            Assert.IsNotNull(player1, "Player 1 is null");
            Assert.IsNotNull(player2, "Player 2 is null");
        }

        [TestMethod]
        public void Bowling_Should_Have_Ten_Frames() {
            var player1 = _sut.AddPlayer("Kamran");

            Assert.AreEqual(10, player1.Frames.Count);
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Frame() {

            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(1);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(6);

            Assert.AreEqual(7, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Gutter_Game() {
            var player1 = _sut.AddPlayer("Kamran");

            BowlMany(20, 0);

            Assert.AreEqual(0, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Spare() {

            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(1);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(9);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(1);

            Assert.AreEqual(11, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Strike() {

            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(10);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(1);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(1);

            Assert.AreEqual(14, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Perfect_Game() {

            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(10);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(30, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(60, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(90, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(120, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(150, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(180, player1.GetScore());

            _sut.Bowl(10);

            Assert.AreEqual(210, player1.GetScore());

            _sut.Bowl(10); // 10th frame, throw 1

            Assert.AreEqual(240, player1.GetScore());

            _sut.Bowl(10); // 10th frame, throw 2

            Assert.AreEqual(270, player1.GetScore());

            _sut.Bowl(10); // 10th frame, throw 3

            Assert.AreEqual(300, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Strike_And_Spare_In_Tenth_Frame() {
            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);

            _sut.Bowl(10); // 10th frame, throw 1

            Assert.AreEqual(240, player1.GetScore());

            _sut.Bowl(4); // 10th frame, throw 2

            Assert.AreEqual(264, player1.GetScore());

            _sut.Bowl(6); // 10th frame, throw 3

            Assert.AreEqual(284, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Spare_And_Strike_In_Tenth_Frame() {
            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);

            _sut.Bowl(1); // 10th frame, throw 1

            Assert.AreEqual(231, player1.GetScore());

            _sut.Bowl(9); // 10th frame, throw 2

            Assert.AreEqual(251, player1.GetScore());

            _sut.Bowl(10); // 10th frame, throw 3

            Assert.AreEqual(271, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Spare_In_Tenth_Frame() {
            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);

            _sut.Bowl(1); // 10th frame, throw 1

            Assert.AreEqual(231, player1.GetScore());

            _sut.Bowl(9); // 10th frame, throw 2

            Assert.AreEqual(251, player1.GetScore());

            _sut.Bowl(1); // 10th frame, throw 3

            Assert.AreEqual(262, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_End_When_Tenth_Frame_Is_Open() {
            var player1 = _sut.AddPlayer("Kamran");

            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);
            _sut.Bowl(10);

            _sut.Bowl(1); // 10th frame, throw 1

            Assert.AreEqual(231, player1.GetScore());

            _sut.Bowl(2); // 10th frame, throw 2

            Assert.AreEqual(247, player1.GetScore());

            Assert.IsTrue(_sut.IsEnded);
        }

        [TestMethod]
        public void Bowling_Should_Allow_Players_To_Take_Turns() {

            var player1 = _sut.AddPlayer("Kamran");
            var player2 = _sut.AddPlayer("Cassie");

            Assert.AreEqual(player1, _sut.CurrentPlayer);

            _sut.Bowl(10); // strike

            Assert.AreEqual(player2, _sut.CurrentPlayer);

            _sut.Bowl(1);
            _sut.Bowl(3);

            Assert.AreEqual(player1, _sut.CurrentPlayer);
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_All_Pins_Available_Before_Throws() {

            var player1 = _sut.AddPlayer("Kamran");

            Assert.AreEqual(10, player1.Frames[0].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_10_Pins_Available_On_Gutter_Ball() {

            var player1 = _sut.AddPlayer("Kamran");

            // gutter ball
            _sut.Bowl(0);

            Assert.AreEqual(10, player1.Frames[0].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_2_Pins_Available_After_Hitting_8_Pins() {

            var player1 = _sut.AddPlayer("Kamran");

            // gutter ball
            _sut.Bowl(8);

            Assert.AreEqual(2, player1.Frames[0].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_No_Pins_Available_After_Hitting_Strike() {

            var player1 = _sut.AddPlayer("Kamran");

            // gutter ball
            _sut.Bowl(10);

            Assert.AreEqual(0, player1.Frames[0].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_10_Pins_Available_In_Tenth_Frame_After_Strike() {

            var player1 = _sut.AddPlayer("Kamran");

            // 10th frame, first throw = strike
            BowlMany(10, 10);

            Assert.AreEqual(10, player1.Frames[9].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_10_Pins_Available_In_Tenth_Frame_After_Two_Strikes() {

            var player1 = _sut.AddPlayer("Kamran");

            // 10th frame, first & second throw = strike
            BowlMany(11, 10);

            Assert.AreEqual(10, player1.Frames[9].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_10_Pins_Available_In_Tenth_Frame_After_Spare() {

            var player1 = _sut.AddPlayer("Kamran");

            // 9th frame, strikes
            BowlMany(9, 10);

            // 10th frame
            _sut.Bowl(1);
            _sut.Bowl(9);

            Assert.AreEqual(10, player1.Frames[9].GetAvailablePins());
        }

        [TestMethod]
        public void Bowling_Frame_Should_Have_Spare_Pins_Available_In_Tenth_Frame_After_Strike_And_Second_Throw() {

            var player1 = _sut.AddPlayer("Kamran");

            // 9th frame, strikes
            BowlMany(9, 10);

            // 10th frame
            _sut.Bowl(10); // first throw
            _sut.Bowl(2); // second throw

            Assert.AreEqual(8, player1.Frames[9].GetAvailablePins());
        }

        private void BowlMany(int throws, int pins) {
            for (var i = 0; i < throws; i++) {
                _sut.Bowl(pins);
            }
        }
    }
}
