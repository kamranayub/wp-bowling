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

            var cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            var releaseDoc = XDocument.Parse(AppResources.ReleaseNotes);
            var releases = from rNode in releaseDoc.Root.Descendants()
                           let versionAttr = rNode.Attribute("version")
                           let notes = from notesNode in rNode.Descendants()
                                       let langAttr = notesNode.Attribute("lang")
                                       where langAttr != null && langAttr.Value == cultureName
                                       select notesNode
                           where notes != null && 
                                 notes.Any() && 
                                 versionAttr != null
                           select
                               new Release()
                               {
                                   Version = versionAttr.Value,
                                   Notes = notes.First().Value
                               };

            ReleaseNotes.AddRange(releases);
        }
    }
}