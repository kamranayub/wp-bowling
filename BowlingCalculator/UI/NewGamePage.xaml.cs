using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BowlingCalculator.UI {
    public partial class NewGamePage : PhoneApplicationPage {
        public NewGamePage() {
            InitializeComponent();
        }

        private void CreateNewGame_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(Player1.Text) || string.IsNullOrEmpty(Player2.Text)) {
                return;
            }

            this.NavigationService.Navigate(
                new Uri("/GamePage.xaml?player1=" + Player1.Text + "&player2=" + Player2.Text, UriKind.Relative));
        }
    }
}