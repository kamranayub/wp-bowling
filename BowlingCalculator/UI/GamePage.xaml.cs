using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using BowlingCalculator.Core;
using BowlingCalculator.UI.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace BowlingCalculator.UI {
    public partial class GamePage : PhoneApplicationPage {

        private readonly GamePageViewModel _pageViewModel;

        public GamePage() {
            InitializeComponent();

            _pageViewModel = new GamePageViewModel();
            DataContext = _pageViewModel;
        }

        private bool _isInitialized;
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            if (_isInitialized) return;

            // Add players to view model
            var queryString = this.NavigationContext.QueryString;

            foreach (var query in queryString) {

                if (query.Key.StartsWith("player")) {
                    _pageViewModel.Game.AddPlayer(query.Value);
                }
            }

            _isInitialized = true;
        }

        private void BowlingFrame_OnTap(object sender, GestureEventArgs e) {
            var border = (Border) sender;
            var frame = border.DataContext as BowlingFrame;

            if (frame != null) {
                
                // Do not show picker if this isn't the current frame
                if (!frame.IsCurrentFrame) {
                    return;
                }

                // Do not show picker if the frame has been completed
                if (frame.IsDone()) {
                    MessageBox.Show("Sorry, the frame has ended!");
                    return;
                }

                // get available pins for the frame
                var pins = frame.GetAvailablePins();

                PinPicker.Items.Clear();
                for (var i = 0; i <= pins; i++) {
                    PinPicker.Items.Add(i);
                }
                
                PinPicker.Open();

                RoutedEventHandler selectionChanged = null;
                selectionChanged = (s, e2) => {
                    PinPicker.Loaded -= selectionChanged;

                    _pageViewModel.Game.Bowl((int)PinPicker.SelectedItem);
                };
                PinPicker.Loaded += selectionChanged;
            }
        }
    }
}