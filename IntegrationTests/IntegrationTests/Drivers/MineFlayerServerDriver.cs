using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using TechTalk.SpecRun;

namespace IntegrationTests.Drivers
{
    public class MineFlayerServerDriver
    {
        private TestRunContext _testRunContext;
        private string _serverPath;
        private Process _process;

        public MineFlayerServerDriver(TestRunContext testRunContext)
        {
            _testRunContext = testRunContext;

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

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        public void Stop()
        {
            _process.Kill();
            _process.Close();
        }
    }
}
