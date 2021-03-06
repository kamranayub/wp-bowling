﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using BowlingCalculator.Core.Resources;

namespace BowlingCalculator.UI.Converters {
    public class ScoreGridTextBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var score = value as string;

            if (score == CoreResources.SpareText || 
                score == CoreResources.StrikeText) {
                return Application.Current.Resources["PhoneForegroundBrush"];
            } else {
                return Application.Current.Resources["PhoneSubtleBrush"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
