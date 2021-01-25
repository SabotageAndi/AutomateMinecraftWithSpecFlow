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
        private readonly MineFlayerServerDriver _mineFlayerServerDriver;

        public Hooks(MinecraftServerDriver minecraftServerDriver, MineFlayerServerDriver mineFlayerServerDriver)
        {
            _minecraftServerDriver = minecraftServerDriver;
            _mineFlayerServerDriver = mineFlayerServerDriver;
        }

        [BeforeScenario()]
        public void StartServers()
        {
            _minecraftServerDriver.Start();
            _mineFlayerServerDriver.Start();
        }

        [AfterScenario()]
        public void StopServers()
        {
            _mineFlayerServerDriver.Stop();
            _minecraftServerDriver.Stop();

        }
    }
}
