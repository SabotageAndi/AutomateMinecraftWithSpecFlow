using System;
using System.Collections.Generic;
using System.Text;
using IntegrationTests.Drivers;
using TechTalk.SpecFlow;

namespace IntegrationTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly MinecraftServerDriver _minecraftServerDriver;

        public Hooks(MinecraftServerDriver minecraftServerDriver)
        {
            _minecraftServerDriver = minecraftServerDriver;
        }

        [BeforeScenario()]
        public void StartServers()
        {
            _minecraftServerDriver.Start();
        }

        [AfterScenario()]
        public void StopServers()
        {
            _minecraftServerDriver.Stop();
        }
    }
}
