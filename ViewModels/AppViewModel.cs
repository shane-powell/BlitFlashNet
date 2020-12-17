using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using BlitFlashNet.GitHubApi;
using BlitFlashNet.Structures;

namespace BlitFlashNet.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private string owner = "Pimoroni";

        private string repo = "32blit-beta";

        private GitHubRelease release = null;

        private GitHubReleaseAsset targetAsset = null;

        public GitHubReleaseAsset TargetAsset
        {
            get => targetAsset;
            set
            {
                targetAsset = value;
                this.OnPropertyChanged();
            }
        }

        public GitHubRelease Release
        {
            get => release;
            set
            {
                release = value; 
                this.OnPropertyChanged();
            }
        }

        public AppViewModel()
        {
            GetLatestFirmwareAsync();
        }

        private async Task GetLatestFirmwareAsync()
        {

            Release = await GitHubApiConnector.GetLatestRelease(owner, repo);

            if (release != null)
            {
                //var assets = await GitHubApiConnector.GetReleaseAssets(release);

                if (release.Assets != null)
                {
                    TargetAsset = release.Assets.FirstOrDefault(a => a.Name.Contains("STM32.zip"));
                }
            }
        }

        private void FlashFirmware()
        {
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;

            // Start a process with the filename or path with filename e.g. "cmd". Please note the 
            //using statemant
            p.StartInfo =
                new ProcessStartInfo(@"C:\Program Files (x86)\STMicroelectronics\Software\DfuSe v3.0.6\Bin\DfuSeCommand.exe",
                    @" -c -d --fn ""firmware.dfu""");
            // add the arguments - Note add "/c" if you want to carry out tge  argument in cmd and  
            // terminate
            // Allows to raise events
            p.EnableRaisingEvents = true;
            //hosted by the application itself to not open a black cmd window
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.CreateNoWindow = true;
            // Eventhander for data
            p.OutputDataReceived += OnOutputData;
            // Eventhandler for error
            p.ErrorDataReceived += OnErrorDataReceived;
            // Eventhandler wich fires when exited
            p.Exited += OnExited;

            // Starts the process
            p.Start();
            //read the output before you wait for exit
            //myProcess.BeginOutputReadLine();

            //Synchronously read the standard output of the spawned process.
            //StreamReader reader = p.StandardOutput;
            //string output = reader.ReadToEnd();

            // Write the redirected output to this application's window.
            //Console.WriteLine(output);

            // wait for the finish - this will block (leave this out if you dont want to wait for 
            // it, so it runs without blocking)
            //p.WaitForExit();
        }

        private void OnOutputData(object sender, EventArgs e)
        {

        }

        //Handle the error
        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.WriteLine(e.Data);
            //do something with your exception
        }

        // Handle Exited event and display process information.
        private void OnExited(object sender, System.EventArgs e)
        {
            Trace.WriteLine("Process exited");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
