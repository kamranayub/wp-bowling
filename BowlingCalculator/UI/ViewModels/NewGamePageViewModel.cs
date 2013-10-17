using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BowlingCalculator.Core.Annotations;
using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class NewGamePageViewModel : BaseViewModel {
        private readonly INavigationService _navigation;
        private string _player1;
        private string _player2;

        public NewGamePageViewModel([NotNull] INavigationService navigation) {
            if (navigation == null) throw new ArgumentNullException("navigation");
            _navigation = navigation;
        }

        public string Player1 {
            get { return _player1; }
            set {
                if (value == _player1) return;
                _player1 = value;
                NotifyOfPropertyChange(() => Player1);
                NotifyOfPropertyChange(() => CanCreateNewGame);
            }
        }

        public string Player2 {
            get { return _player2; }
            set {
                if (value == _player2) return;
                _player2 = value;
                NotifyOfPropertyChange(() => Player2);
                NotifyOfPropertyChange(() => CanCreateNewGame);
            }
        }

        public bool CanCreateNewGame {
            get {
                return !string.IsNullOrEmpty(Player1) ||
                       (!string.IsNullOrEmpty(Player1) && !string.IsNullOrEmpty(Player2));
            }
        }

        public void CreateNewGame() {
            
            _navigation.UriFor<GamePageViewModel>()
                       .WithParam(p => p.Player1, Player1)
                       .WithParam(p => p.Player2, Player2)
                       .Navigate();            
        }
    }
}
