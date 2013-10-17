using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using BowlingCalculator.UI.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BowlingCalculator.UI {
    public partial class App : Application {
        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App() {           
            InitializeComponent();            
        }
    }
}