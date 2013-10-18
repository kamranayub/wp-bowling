using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class PinPickerViewModel : BaseViewModel {

        public int AvailablePins { get; set; }

        public int? SelectedPins { get; set; }

        public IObservableCollection<int> Pins { get; set; }

        public PinPickerViewModel() {
            Pins = new BindableCollection<int>();

            if (Execute.InDesignMode) {
                for (var i = 0; i <= 10; i++)
                    Pins.Add(i);
            }
        }

        protected override void OnActivate() {
            base.OnActivate();

            for (var i = 0; i <= AvailablePins; i++) {
                Pins.Add(i);
            }
        }

        public void SelectPins(int pins) {
            SelectedPins = pins;

            TryClose();
        }
    }
}