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

    /// <summary>
    /// A Round Button that automatically inverts an image as a selected state.
    /// </summary>
    /// <remarks>
    /// Source: http://www.jayway.com/2011/02/03/create-a-round-button-control-for-windows-phone-7/
    /// </remarks>
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
