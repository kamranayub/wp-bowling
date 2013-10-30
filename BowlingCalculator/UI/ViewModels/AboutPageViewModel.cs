using System;
using System.Linq;
using System.Reflection;
using BowlingCalculator.Core.Annotations;
using Caliburn.Micro;
using Microsoft.Phone.Tasks;

namespace BowlingCalculator.UI.ViewModels {
    public class AboutPageViewModel : BaseViewModel {
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _events;
        private string _version;

        public AboutPageViewModel() : this(null, null) {
            
        }

        public AboutPageViewModel(INavigationService navigation, IEventAggregator events) {
            _navigation = navigation;
            _events = events;

            if (Execute.InDesignMode) {
                Version = "1.0.0";
            }

            Website = "http://wp-bowling.tumblr.com";
            ProjectSite = "http://github.com/kamranayub/wp-bowling";
            FeedbackSite = "http://wp-bowling.uservoice.com";
            SupportEmail = "tickets@wp-bowling.uservoice.com";            
        }

        public string Version {
            get { return _version; }
            set {
                if (value == _version) return;
                _version = value;
                NotifyOfPropertyChange(() => Version);
            }
        }

        public string Website { get; set; }

        public string ProjectSite { get; set; }

        public string FeedbackSite { get; set; }

        public string SupportEmail { get; set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            Version = GetAssemblyVersion().ToString();
        }

        public void OpenChangelog() {
            _navigation.UriFor<ChangelogPageViewModel>().Navigate();
        }

        public void OpenRate() {
            _events.RequestTask<MarketplaceReviewTask>();
        }

        public void OpenWebsite() {
            _events.RequestTask<WebBrowserTask>(t => t.Uri = new Uri(Website));
        }

        public void OpenFeedbackSite() {
            _events.RequestTask<WebBrowserTask>(t => t.Uri = new Uri(FeedbackSite));
        }

        public void OpenSupportEmail() {
            _events.RequestTask<EmailComposeTask>(t =>
                {
                    t.Subject = "Issue with Bowling Calc app";
                    t.To = SupportEmail;                    
                });
        }

        public void OpenProjectSite() {
            _events.RequestTask<WebBrowserTask>(t => t.Uri = new Uri(ProjectSite));
        }

        private Version GetAssemblyVersion() {
            var versionString = Assembly.GetExecutingAssembly().GetCustomAttributes(false)
                .OfType<AssemblyFileVersionAttribute>()
                .First()
                .Version;

            return System.Version.Parse(versionString);
        }
    }
}