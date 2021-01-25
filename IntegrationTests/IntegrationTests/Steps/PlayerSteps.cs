using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace IntegrationTests.Steps
{
    [Binding]
    class PlayerSteps
    {
        [Given(@"there is the '(.*)' at X:(.*),Z:(.*)")]
        public void GivenThereIsTheAtXZ(string p0, int p1, int p2)
        {
        }

        [Given(@"there is the '(.*)' at X:(.*),Z:(.*) pointing to '(.*)'")]
        public void GivenThereIsTheAtXZPointingTo(string p0, int p1, int p2, string p3)
        {
        }

        [Given(@"there is the '(.*)' at X:(.*),Y:(.*),Z:(.*)")]
        public void GivenThereIsTheAtXYZ(string p0, int p1, int p2, int p3)
        {
        }

        [When(@"a player puts an item into '(.*)'")]
        public void WhenAPlayerPutsAnItemInto(string p0)
        {
        }

        [Then(@"it appears in '(.*)'")]
        public void ThenItAppearsIn(string p0)
        {
        }

    }
}
