using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
using BowlingCalculator.UI.Controls;
using BowlingCalculator.UI.ViewModels;
using BugSense;
using Caliburn.Micro.BindableAppBar;
using Microsoft.Phone.Shell;

namespace BowlingCalculator.UI {
	using System;
	using System.Collections.Generic;
	using System.Windows.Controls;
	using Microsoft.Phone.Controls;
	using Caliburn.Micro;

    /// <summary>
    /// Bowling custom phone app bootstrapper
    /// </summary>
    public class BowlingBootstrapperBase : PhoneBootstrapperBase {

        /// <summary>
        /// Enable transitions
        /// </summary>
        /// <returns></returns>
        protected override PhoneApplicationFrame CreatePhoneApplicationFrame() {
            return new TransitionFrame();
        }
    }

    public class AppBootstrapper : BowlingBootstrapperBase
	{
		PhoneContainer container;

		public AppBootstrapper()
		{
			Start();
		}

		protected override void Configure()
		{
			container = new PhoneContainer();
			if (!Execute.InDesignMode)
				container.RegisterPhoneServices(RootFrame);

			container.Singleton<MainPageViewModel>();
            container.Singleton<GamePageViewModel>();
            // works around issue with storage handler
            // see: https://caliburnmicro.codeplex.com/workitem/340
            container.Singleton<AddPlayerPageViewModel>();

		    container.PerRequest<PinPickerViewModel>();
            container.PerRequest<AboutPageViewModel>();
            container.PerRequest<ChangelogPageViewModel>();
		    	    

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
		            else {
		                MessageBox.Show(
		                    "An error report will be sent so we can improve the app, please try running the app again!",
		                    "An error occurred", MessageBoxButton.OK);
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

        protected override void OnUnhandledException(object sender, System.Windows.ApplicationUnhandledExceptionEventArgs e) {
            if (Debugger.IsAttached) {
                Debugger.Break();
            }
        }

        private void HandleFastResume() {
	        bool wasReset = false;

	        RootFrame.Navigating += (s, e) =>
	            {
                    // first call will be a Reset
	                if (e.NavigationMode == NavigationMode.Reset) {
                        wasReset = true;
	                }
                    // next call will be New (after the Reset)
                    // only cancel if we're heading to MainPage
                    else if (e.NavigationMode == NavigationMode.New && wasReset && e.Uri.ToString().Contains("MainPage.xaml")) {
                        e.Cancel = true;
                        wasReset = false;
                    }
	            };
		}

		protected override object GetInstance(Type service, string key)
		{
			var instance = container.GetInstance(service, key);
			if (instance != null)
				return instance;

			throw new InvalidOperationException("Could not locate any instances.");
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return container.GetAllInstances(service);
		}

		protected override void BuildUp(object instance)
		{
			container.BuildUp(instance);
		}

		static void AddCustomConventions() {
            // App Bar Conventions
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");

		    ConventionManager.AddElementConvention<RoundButton>(Control.IsEnabledProperty, "DataContext", "Click");
			ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
				(viewModelType, path, property, element, convention) => {
					if (ConventionManager
						.GetElementConvention(typeof(ItemsControl))
						.ApplyBinding(viewModelType, path, property, element, convention))
					{
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
						.ApplyBinding(viewModelType, path, property, element, convention))
					{
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