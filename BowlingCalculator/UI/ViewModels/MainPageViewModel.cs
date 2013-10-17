using System;
using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class MainPageViewModel : BaseViewModel {
        private readonly INavigationService _navigation;

        public MainPageViewModel(INavigationService navigation) {
            if (navigation == null) throw new ArgumentNullException("navigation");
            _navigation = navigation;
        }

        public void NewGame() {
            _navigation.UriFor<NewGamePageViewModel>().Navigate();
        }
    }
}