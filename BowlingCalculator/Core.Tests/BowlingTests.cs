using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BowlingCalculator.Core.Tests {
    [TestClass]
    public class BowlingTests {

        private Bowling _sut;

        [TestInitialize]
        public void Setup() {
            _sut = new Bowling();
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

            _sut.Start();

            _sut.Bowl(1);

            Assert.AreEqual(0, player1.GetScore());

            _sut.Bowl(6);

            Assert.AreEqual(7, player1.GetScore());
        }

        [TestMethod]
        public void Bowling_Should_Allow_Player_To_Bowl_A_Spare() {

            var player1 = _sut.AddPlayer("Kamran");

            _sut.Start();

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

            _sut.Start();

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

            _sut.Start();

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
        public void Bowling_Should_Allow_Player_To_Bowl_A_Spare_And_Strike_In_Tenth_Frame() {
            var player1 = _sut.AddPlayer("Kamran");

            _sut.Start();

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
        public void Bowling_Should_Allow_Players_To_Take_Turns() {

            var player1 = _sut.AddPlayer("Kamran");
            var player2 = _sut.AddPlayer("Cassie");

            _sut.Start();

            Assert.AreEqual(player1, _sut.CurrentPlayer);

            _sut.Bowl(10); // strike

            Assert.AreEqual(player2, _sut.CurrentPlayer);

            _sut.Bowl(1);
            _sut.Bowl(3);

            Assert.AreEqual(player1, _sut.CurrentPlayer);
        }
    }
}
