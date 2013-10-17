using System.Windows.Navigation;
using BowlingCalculator.UI.ViewModels;

namespace BowlingCalculator.UI {
	using System;
	using System.Collections.Generic;
	using System.Windows.Controls;
	using Microsoft.Phone.Controls;
	using Caliburn.Micro;

	public class AppBootstrapper : PhoneBootstrapperBase
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

			container.PerRequest<MainPageViewModel>();
		    container.PerRequest<NewGamePageViewModel>();
		    container.PerRequest<GamePageViewModel>();

			AddCustomConventions();

		    HandleFastResume();
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
                    else if (e.NavigationMode == NavigationMode.New && wasReset) {
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

		static void AddCustomConventions()
		{
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
}