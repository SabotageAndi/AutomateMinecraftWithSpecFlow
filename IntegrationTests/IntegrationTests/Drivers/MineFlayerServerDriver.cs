using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Polly;
using TechTalk.SpecRun;

namespace IntegrationTests.Drivers
{
    public class MineFlayerServerDriver
    {
        private TestRunContext _testRunContext;
        private readonly MineFlayerDriver _mineFlayerDriver;
        private string _serverPath;
        private Process _process;

        public MineFlayerServerDriver(TestRunContext testRunContext, MineFlayerDriver mineFlayerDriver)
        {
            _testRunContext = testRunContext;
            _mineFlayerDriver = mineFlayerDriver;

            _serverPath = Path.Combine(_testRunContext.TestDirectory, "..", "..", "..", "..", "..", "MineFlayerPart");
        }

        public void Start()
        {
            var processStartInfo = new ProcessStartInfo("node", $".\\index.js");
            processStartInfo.WorkingDirectory = _serverPath;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            _process = new Process() { StartInfo = processStartInfo };
            _process.Start();

            var policy = Polly.Policy.HandleResult<bool>(v => !v).RetryForever();

            policy.Execute(() => _mineFlayerDriver.GetStatus());

            
        }

        public void Stop()
        {
            _process.Kill();
            _process.Close();
        }
    }
}
