using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace IntegrationTests.Steps
{
    [Binding]
    class ServerSteps
    {
        private readonly WorldContext _worldContext;

        public ServerSteps(WorldContext worldContext)
        {
            _worldContext = worldContext;
        }

        [Given(@"a creative superflat world on level '(.*)'")]
        public void GivenACreativeSuperflatWorldOnLevel(int yLevel)
        {
            _worldContext.YLevel = yLevel;
        }

    }
}
