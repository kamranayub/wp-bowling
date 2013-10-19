using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BowlingCalculator.Core.Annotations;
using BowlingCalculator.Core.Messages;
using BowlingCalculator.UI.Support;
using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class AddPlayerPageViewModel : BaseViewModel {
        private readonly INavigationService _navigation;
        private string _player;
        private IObservableCollection<string> _recentPlayers;

        public AddPlayerPageViewModel() : this(null) {
            if (Execute.InDesignMode) {
                RecentPlayers.Add("Kamran");
                RecentPlayers.Add("Cassie");
            }
        }

        public AddPlayerPageViewModel(INavigationService navigation) {
            _navigation = navigation;

            RecentPlayers = new BindableCollection<string>();
        }

        public IObservableCollection<string> RecentPlayers {
            get { return _recentPlayers; }
            set {
                if (Equals(value, _recentPlayers)) return;
                _recentPlayers = value;
                NotifyOfPropertyChange(() => RecentPlayers);
            }
        }

        public string Player {
            get { return _player; }
            set {
                if (value == _player) return;
                _player = value;
                NotifyOfPropertyChange(() => Player);
                NotifyOfPropertyChange(() => CanAddPlayer);
            }
        }
        
        public bool CanAddPlayer {
            get {
                return !string.IsNullOrEmpty(Player);
            }
        }

        public void AddPlayer() {
            AddPlayer(Player);
            Player = null;
        }

        public void AddPlayer(string name) {
            // last 5 entries
            if (RecentPlayers.Contains(name)) {
                RecentPlayers.Remove(name);
            }

            RecentPlayers.Insert(0, name);

            if (RecentPlayers.Count > 5) {
                RecentPlayers.RemoveRange(RecentPlayers.Skip(5).ToList());
            }

            _navigation.GoBackThenPublish(this, new RequestAddPlayerMessage(name));            
        }
    }

    public class AddPlayerStorageHandler : StorageHandler<AddPlayerPageViewModel> {
        public override void Configure() {
            Property(vm => vm.RecentPlayers)
                .InPhoneState()
                .InAppSettings()
                .RestoreAfterViewLoad();
        }
    }
}
