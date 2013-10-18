using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Caliburn.Micro;

namespace BowlingCalculator.UI.Support {
    public static class NavigationExtensions {
        /// <summary>
        /// Performs a back navigation and once finished, executes the specified action
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="closeable"></param>
        /// <param name="message"></param>
        public static void GoBackThenPublish(this INavigationService nav, IClose closeable, object message) {
            NavigatedEventHandler afterNavigation = null;

            // Handles after navigation
            afterNavigation = (s, e) => {
                // Remove handler
                nav.Navigated -= afterNavigation;

                // Get event aggregator
                var events = IoC.Get<IEventAggregator>();

                // Publish event
                if (events != null) {
                    events.Publish(message);
                }
            };

            // Attach handler
            nav.Navigated += afterNavigation;

            // Go back
            nav.GoBack();
        }

    }
}
