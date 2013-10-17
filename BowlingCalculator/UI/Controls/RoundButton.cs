using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BowlingCalculator.UI.Controls {
    public class RoundButton : Button {

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(RoundButton), null);

        public RoundButton()
            : base() {
            DefaultStyleKey = typeof(RoundButton);
        }


        [Description("The image displayed by the button in dark theme (and in normal mode)"), Category("Common Properties")]
        public ImageSource Image {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }
}
