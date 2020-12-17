using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using BlitFlashNet.GitHubApi;
using BlitFlashNet.Structures;
using BlitFlashNet.Util;

namespace BlitFlashNet.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private string owner = "Pimoroni";

        private string repo = "32blit-beta";

        private GitHubRelease release = null;

        private GitHubReleaseAsset targetAsset = null;

        private readonly RelayCommand DownloadFirmwareCommand = null;

        private readonly RelayCommand FlashFirmwareCommand = null;

        private readonly string DownloadPath = @$"{Environment.CurrentDirectory}\release.zip";

        private readonly string firmwarePath = @$"{Environment.CurrentDirectory}\bin\firmware.dfu";


        private int percentageComplete = 0;

        private Visibility progressBarVisible = Visibility.Collapsed;

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

        public RelayCommand DownloadFirmwareCommand1 => DownloadFirmwareCommand;

        public RelayCommand FlashFirmwareCommand1 => FlashFirmwareCommand;

        public int PercentageComplete
        {
            get => percentageComplete;
            set
            {
                percentageComplete = value; 
                this.OnPropertyChanged();
            }
        }

        public Visibility ProgressBarVisible
        {
            get => progressBarVisible;
            set
            {
                progressBarVisible = value;
                this.OnPropertyChanged();
            }
        }

        public AppViewModel()
        {
            this.DownloadFirmwareCommand = new RelayCommand(this.DownloadFirmware);

            this.FlashFirmwareCommand = new RelayCommand(this.FlashFirmware);

            GetLatestFirmwareAsync();
        }

        private void FlashFirmware(object obj)
        {
            this.FlashFirmware();
        }

        private void DownloadFirmware(object obj)
        {
            if (File.Exists(DownloadPath))
            {
                File.Delete(DownloadPath);
            }

            if (this.targetAsset != null)
            {
                this.ProgressBarVisible = Visibility.Visible;

                FileDownLoader.DownloadFile(this.targetAsset.DownloadUrl, DownloadPath, FirmwareDownloadProgressChanged, FirmwareDownloadCompleted);
            }
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
            if (File.Exists(firmwarePath))
            {
                Process p = new Process();
                p.StartInfo.RedirectStandardOutput = true;

                // Start a process with the filename or path with filename e.g. "cmd". Please note the 
                //using statemant
                p.StartInfo =
                    new ProcessStartInfo(
                        @$"C:\Program Files (x86)\STMicroelectronics\Software\DfuSe v3.0.6\Bin\DfuSeCommand.exe",
                        $@" -c -d --fn ""{firmwarePath}""");

                // Allows to raise events
                p.EnableRaisingEvents = true;
                //p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                // Eventhander for data
                p.OutputDataReceived += OnOutputData;
                // Eventhandler for error
                p.ErrorDataReceived += OnErrorDataReceived;
                // Eventhandler wich fires when exited
                p.Exited += OnExited;

                // Starts the process
                p.Start();

                //StreamReader reader = p.StandardOutput;

                //p.BeginOutputReadLine();

                //string output = reader.ReadToEnd();

                //Console.WriteLine(output);

            }
            else
            {
                MessageBox.Show("Firmware not found");
            }
        }

        private void OnOutputData(object sender, EventArgs e)
        {

        }

        //Handle the error
        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.WriteLine(e.Data);
        }

        // Handle Exited event and display process information.
        private void OnExited(object sender, System.EventArgs e)
        {
            MessageBox.Show("Firmware upload complete");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void FirmwareDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            var percentageDownloaded = e.BytesReceived / e.TotalBytesToReceive * 100;
            PercentageComplete = Convert.ToInt32(percentageDownloaded);

        }
        void FirmwareDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.ProgressBarVisible = Visibility.Collapsed;
            if (File.Exists(DownloadPath))
            {
                ZipFile.ExtractToDirectory(DownloadPath, Environment.CurrentDirectory, true);
                if (File.Exists(firmwarePath))
                {
                    MessageBox.Show("Downloaded Firmware. Ready to Flash");
                    return;
                }
            }

            MessageBox.Show("Failed to download firmware");
        }
    }
}
