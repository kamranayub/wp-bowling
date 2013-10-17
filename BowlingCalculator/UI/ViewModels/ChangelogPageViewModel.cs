using System.Linq;
using System.Xml.Linq;
using BowlingCalculator.UI.Models;
using BowlingCalculator.UI.Resources;
using Caliburn.Micro;

namespace BowlingCalculator.UI.ViewModels {
    public class ChangelogPageViewModel : BaseViewModel {

        public BindableCollection<Release> ReleaseNotes { get; set; }

        public ChangelogPageViewModel() {
            ReleaseNotes = new BindableCollection<Release>();

            if (Execute.InDesignMode) {
                ReleaseNotes.AddRange(FakeData.ReleaseNotes);
            }
        }

        /// <summary>
        /// Load release notes
        /// </summary>
        /// <param name="view"></param>
        /// <remarks>Use OnViewReady to avoid any UI blocking when loading the view</remarks>
        protected override void OnViewReady(object view) {
            base.OnViewReady(view);

            var releaseDoc = XDocument.Parse(AppResources.ReleaseNotes);
            var releases = from rNode in releaseDoc.Root.Descendants()
                           select
                               new Release()
                               {
                                   Version = rNode.Attribute("version").Value,
                                   Notes = rNode.Value
                               };

            ReleaseNotes.AddRange(releases);
        }
    }
}