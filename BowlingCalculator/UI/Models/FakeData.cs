using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingCalculator.UI.Models {
    public class FakeData {

        public static IList<Release> ReleaseNotes = new List<Release>()
            {
                new Release() { Version = "1.0.0", Notes = "Testing this foolish release note thing"},
                new Release() { Version = "0.0.1", Notes = "More testing for SCIENCE"}
            }; 

    }
}
