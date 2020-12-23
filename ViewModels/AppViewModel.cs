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
        public enum FlashTool
        {
            BlitTool,
            DfuSe
        }

        private const string owner = "Pimoroni";

        private const string repo = "32blit-beta";

        private string dFusePath = @"C:\Program Files (x86)\STMicroelectronics\Software\DfuSe v3.0.6\Bin\DfuSeCommand.exe";

        private const string dfuseFilename = "DfuSeCommand.exe";

        private GitHubRelease release = null;

        private List<GitHubRelease> releases = null;

        private GitHubReleaseAsset targetAsset = null;

        private readonly RelayCommand DownloadFirmwareCommand = null;

        private readonly RelayCommand FlashFirmwareCommand = null;

        private readonly string DownloadPath = @$"{Environment.CurrentDirectory}\release.zip";

        private readonly string dfuPath = @$"{Environment.CurrentDirectory}\bin\firmware.dfu";

        private readonly List<string> possibleDfuseLocations = new List<string>() { $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\STMicroelectronics\Software\", $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\STMicroelectronics\Software\" };

        private bool dfuseFound = false;

        private bool isFlashing = false;

        private bool interfaceEnabled = true;

        private string flashOutput = string.Empty;

        private CommandLineTools commandLineTools = null;

        private FlashTool selectedFlashTool = FlashTool.BlitTool;

        //private string dfuseInstallLocation =
        //    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles) + @"\STMicroelectronics\Software\";


        private int percentageComplete = 0;

        private Visibility progressBarVisible = Visibility.Collapsed;

        public List<FlashTool> FlashTools
        {
            get
            {
                return Enum.GetValues(typeof(FlashTool)).Cast<FlashTool>().ToList<FlashTool>();
            }
        }

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

                this.UpdateTargetAsset();
            }
        }

        private void UpdateTargetAsset()
        {
            if (release != null)
            {
                if (release.Assets != null)
                {
                    TargetAsset = release.Assets.FirstOrDefault(a => a.Name.Contains("STM32.zip"));
                }
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

        public bool DfuseFound
        {
            get => dfuseFound;
            set
            {
                dfuseFound = value; 
                this.OnPropertyChanged();
            }
        }

        public bool IsFlashing
        {
            get => isFlashing;
            set
            {
                isFlashing = value; 
                this.OnPropertyChanged();
            }
        }

        public bool InterfaceEnabled
        {
            get => interfaceEnabled;
            set
            {
                interfaceEnabled = value; 
                this.OnPropertyChanged();
            }
        }

        public string FlashOutput
        {
            get => flashOutput;
            set
            {
                flashOutput = value; 
                this.OnPropertyChanged();
            }
        }

        public List<GitHubRelease> Releases
        {
            get => releases;
            set
            {
                releases = value; 
                this.OnPropertyChanged();
            }
        }

        public FlashTool SelectedFlashTool
        {
            get => selectedFlashTool;
            set
            {
                selectedFlashTool = value; 
                this.OnPropertyChanged();
            }
        }

        public AppViewModel()
        {
            this.TryFindDfuse();

            this.DownloadFirmwareCommand = new RelayCommand(this.DownloadFirmware);

            this.FlashFirmwareCommand = new RelayCommand(this.FlashFirmware);

            this.commandLineTools = new CommandLineTools(this.ProcessCommandLineMessage, this.ProcessCommandLineFinished);

            //GetLatestFirmwareAsync();
            GetAllFirmwareAsync();
        }

        private void TryFindDfuse()
        {
            foreach (var possibleDfuseLocation in this.possibleDfuseLocations)
            {
                if (Directory.Exists(possibleDfuseLocation))
                {
                    var path = this.SearchForFileInDirectoy(possibleDfuseLocation, dfuseFilename);

                    if (path != null)
                    {
                        this.dFusePath = path;
                        this.DfuseFound = true;
                    }

                    return;
                }
            }
        }

        private string SearchForFileInDirectoy(string possibleDfuseLocation, string filename)
        {
            var path = $"{possibleDfuseLocation}{filename}";

            if (File.Exists(path))
            {
                return path;
            }
            else
            {
               var directories = Directory.GetDirectories(possibleDfuseLocation);

               if (directories != null)
               {
                   foreach (var directory in directories)
                   {
                      var foundPath = this.SearchForFileInDirectoy(directory + "\\", filename);
                      if (foundPath != null)
                      {
                          return foundPath;
                      }
                   }
               }
            }

            return null;
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
                this.PercentageComplete = 0;
                this.ProgressBarVisible = Visibility.Visible;

                FileDownLoader.DownloadFile(this.targetAsset.DownloadUrl, DownloadPath, FirmwareDownloadProgressChanged, FirmwareDownloadCompleted);
            }
        }

        private async Task GetLatestFirmwareAsync()
        {
            this.InterfaceEnabled = false;

            try
            {
                
                Release = await GitHubApiConnector.GetLatestRelease(owner, repo);

                if (release != null)
                {
                    if (release.Assets != null)
                    {
                        TargetAsset = release.Assets.FirstOrDefault(a => a.Name.Contains("STM32.zip"));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                this.InterfaceEnabled = true;
            }
        }

        private async Task GetAllFirmwareAsync()
        {
            this.InterfaceEnabled = false;

            try
            {

                Releases = await GitHubApiConnector.GetAllReleases(owner, repo);

                if (releases.Any())
                {
                    Release = Releases.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                this.InterfaceEnabled = true;
            }
        }

        private void FlashFirmware()
        {
            var blitFlashPath = $@"{Environment.CurrentDirectory }\bin\firmware-update-{release.TagName}.blit";

            if ((selectedFlashTool == FlashTool.DfuSe && File.Exists(dfuPath)) || selectedFlashTool == FlashTool.BlitTool && File.Exists(blitFlashPath))
            {
                try
                {
                    this.FlashOutput = string.Empty;
                    this.IsFlashing = true;
                    this.InterfaceEnabled = false;

                    switch (SelectedFlashTool)
                    {
                        case FlashTool.BlitTool:
                            this.commandLineTools.RunCommandLineApp("cmd.exe", $@"/c 32blit flash flash --file {blitFlashPath}");
                            break;
                        case FlashTool.DfuSe:
                            this.commandLineTools.RunCommandLineApp(dFusePath, $@" -c -d --fn ""{dfuPath}""");
                            break;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    this.InterfaceEnabled = true;
                }


            }
            else
            {
                MessageBox.Show("Firmware not found");
            }
        }

        private void ProcessCommandLineMessage(string message)
        {
            this.FlashOutput += $"{Environment.NewLine}{message}";
        }

        // Handle Exited event and display process information.
        private void ProcessCommandLineFinished()
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

            var percentageDownloaded = ((double)e.BytesReceived / e.TotalBytesToReceive) * 100;
            PercentageComplete = Convert.ToInt32(percentageDownloaded);

        }
        void FirmwareDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.ProgressBarVisible = Visibility.Collapsed;
            if (File.Exists(DownloadPath))
            {
                ZipFile.ExtractToDirectory(DownloadPath, Environment.CurrentDirectory, true);
                if (File.Exists(dfuPath))
                {
                    MessageBox.Show("Downloaded Firmware. Ready to Flash");
                    return;
                }
            }

            MessageBox.Show("Failed to download firmware");
        }
    }
}
