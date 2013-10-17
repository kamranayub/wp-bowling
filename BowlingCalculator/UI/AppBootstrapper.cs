using BowlingCalculator.UI.Controls;
using BowlingCalculator.UI.ViewModels;
using BugSense;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BowlingCalculator.UI {

    public class AppBootstrapper : PhoneBootstrapperBase {
        PhoneContainer container;

        public AppBootstrapper() {
            Start();
        }

        protected override void Configure() {
            container = new PhoneContainer();
            if (!Execute.InDesignMode)
                container.RegisterPhoneServices(RootFrame);

            container.PerRequest<MainPageViewModel>();
            container.PerRequest<AboutPageViewModel>();
            container.PerRequest<ChangelogPageViewModel>();
            container.PerRequest<NewGamePageViewModel>();
            container.PerRequest<GamePageViewModel>();

            AddCustomConventions();

            EnableDebugging();

            HandleFastResume();

#if DEBUG
            LogManager.GetLog = type => new DebugLogger(type);
#else
            BugSenseHandler.Instance.InitAndStartSession(Application, "");
		    BugSenseHandler.Instance.UnhandledException += (sender, args) =>
		        {
		            if (Debugger.IsAttached) {
		                Debugger.Break();
		            }
		        };

		    LogManager.GetLog = type => new BugSenseLogger(type);
#endif
        }

        protected void EnableDebugging() {
            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached) {
                // Display the current frame rate counters.
                Application.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        private void HandleFastResume() {
            bool wasReset = false;

            RootFrame.Navigating += (s, e) => {
                // first call will be a Reset
                if (e.NavigationMode == NavigationMode.Reset) {
                    wasReset = true;
                }
                    // next call will be New (after the Reset)
                else if (e.NavigationMode == NavigationMode.New && wasReset) {
                    e.Cancel = true;
                    wasReset = false;
                }
            };
        }

        protected override object GetInstance(Type service, string key) {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }

        static void AddCustomConventions() {
            ConventionManager.AddElementConvention<RoundButton>(Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) => {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention)) {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };

            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) => {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention)) {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };
        }
    }

    public class DebugLogger : ILog {
        private readonly Type _type;
        private const string DateFormat = "hh:mm:ss.ffff";

        public DebugLogger(Type type) {
            _type = type;
        }

        public void Info(string format, params object[] args) {

            Debug.WriteLine(String.Format("{0} INFO [Thread:{2}][{1}] - ", DateTime.Now.ToString(DateFormat), _type.Name, System.Threading.Thread.CurrentThread.ManagedThreadId) + format, args);

        }

        public void Warn(string format, params object[] args) {

            Debug.WriteLine(String.Format("{0} WARN [Thread:{2}][{1}] - ", DateTime.Now.ToString(DateFormat), _type.Name, System.Threading.Thread.CurrentThread.ManagedThreadId) + format, args);

        }

        public void Error(Exception exception) {

            Debug.WriteLine(String.Format("{0} ERROR [Thread:{2}][{1}] - ", DateTime.Now.ToString(DateFormat), _type.Name, System.Threading.Thread.CurrentThread.ManagedThreadId) + exception.Message);
            Debug.WriteLine(exception.StackTrace);

        }
    }

    public class BugSenseLogger : ILog {
        private readonly Type _type;

        public BugSenseLogger(Type type) {
            _type = type;
        }

        public void Info(string format, params object[] args) {
            // do nothing
        }

        public void Warn(string format, params object[] args) {
            // do nothing
        }

        public void Error(Exception exception) {
            BugSenseHandler.Instance.LogException(exception, "sender", _type.FullName);
        }
    }
}