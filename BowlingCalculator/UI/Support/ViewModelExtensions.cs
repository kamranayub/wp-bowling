using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BowlingCalculator.UI.Controls;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Action = System.Action;

namespace BowlingCalculator.UI {
    public static class ViewModelExtensions {

        public static BowlingCustomMessageBox DismissableDialogFor<TViewModel>(this IScreen screen,
            Action<BowlingCustomMessageBox> configureMessageBox = null,
            Action<TViewModel> configureViewModel = null,
            Action<TViewModel, DismissedEventArgs> onDismissed = null) where TViewModel : class {

            var dialog = new BowlingCustomMessageBox();
            // use custom message box based on toolkit CustomMessageBox
            var pinPickerView = ViewLocator.LocateForModelType(typeof(TViewModel), null, null);
            var pinPickerModel = ViewModelLocator.LocateForView(pinPickerView) as TViewModel;
            var pinConductor = new PinPickerBoxConductor<TViewModel>(dialog);

            if (pinPickerModel == null || pinPickerView == null) {
                throw new InvalidOperationException("Could not find view model or view for Pin Picker");
            }

            if (configureViewModel != null)
                configureViewModel(pinPickerModel);

            // conduct            
            pinConductor.ActivateItem(pinPickerModel);

            // this does not activate the VM lifecycle (Activate, Deactivate, etc.)
            ViewModelBinder.Bind(pinPickerModel, pinPickerView, null);

            // set content
            dialog.Content = pinPickerView;

            if (configureMessageBox != null)
                configureMessageBox(dialog);

            EventHandler<DismissedEventArgs> onDialogDismissed = null;
            onDialogDismissed = (sender, args) =>
                {
                    // remove event handler
                    dialog.Dismissed -= onDialogDismissed;

                    // make sure we don't keep an instance around
                    pinConductor.TryClose();

                    // callback
                    if (onDismissed != null)
                        onDismissed(pinPickerModel, args);
                };

            dialog.Dismissed += onDialogDismissed;

            return dialog;
        }

        /// <summary>
        /// Implements a Caliburn conductor that dismisses a dialog box when its child is deactivated and activates it when its activated
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        private class PinPickerBoxConductor<TViewModel> : Conductor<TViewModel> where TViewModel : class {
            private readonly BowlingCustomMessageBox _messageBox;

            public PinPickerBoxConductor(BowlingCustomMessageBox messageBox) {
                _messageBox = messageBox;
            }

            public override void ActivateItem(TViewModel item) {
                base.ActivateItem(item);

                if (item is IActivate) {
                    (item as IActivate).Activate();
                }
            }

            public override void DeactivateItem(TViewModel item, bool close) {
                base.DeactivateItem(item, close);

                if (close && _messageBox != null) {
                    _messageBox.Dismiss();                    
                }
            }
        }
    }
}
