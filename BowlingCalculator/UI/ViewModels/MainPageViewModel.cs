using System;
using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class MainPageViewModel : BaseViewModel {

        public MainPageViewModel(Func<GamePageViewModel> gamePageFactory) {            
            GamePage = gamePageFactory();

            // Initiate the viewmodel lifecycle
            GamePage.ActivateWith(this);
            GamePage.DeactivateWith(this);
        }

        /// <summary>
        /// Caliburn will bind the view associated with this view model to any ContentControl
        /// </summary>
        public GamePageViewModel GamePage { get; set; }
    }
}