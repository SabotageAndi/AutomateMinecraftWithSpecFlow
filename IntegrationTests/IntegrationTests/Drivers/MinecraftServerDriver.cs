using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using TechTalk.SpecRun;

namespace IntegrationTests.Drivers
{
    public class MinecraftServerDriver
    {
        private readonly TestRunContext _testRunContext;
        private string _serverJarPath;
        private string _worldFolder;
        private string _worldBackupFolder;
        private Process _process;

        public MinecraftServerDriver(TestRunContext testRunContext)
        {
            _testRunContext = testRunContext;

            _serverJarPath = Path.Combine(_testRunContext.TestDirectory, "..", "..", "..", "..", "..", "Server");
            _worldFolder = Path.Combine(_serverJarPath, "world");
            _worldBackupFolder = Path.Combine(_serverJarPath, "worldBackup");
        }

        public void Start()
        {
            if (Directory.Exists(_worldBackupFolder))
            {
                if (Directory.Exists(_worldFolder))
                {
                    Directory.Delete(_worldFolder, true);
                    DirectoryCopy(_worldBackupFolder, _worldFolder, true);
                }
            }


            var processStartInfo = new ProcessStartInfo("java", $"-Xmx1024M -Xms1024M -jar .\\server.jar nogui");
            processStartInfo.WorkingDirectory = _serverJarPath;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            _process = new Process() { StartInfo = processStartInfo};
            _process.Start();

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        public void Stop()
        {
            _process.StandardInput.WriteLine("/stop");
            _process.WaitForExit(TimeSpan.FromSeconds(10).Milliseconds);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
