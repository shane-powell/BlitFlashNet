using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlitFlashNet.Util
{
    internal class CommandLineTools
    {
        private readonly Action<string> processUpdateDelegate;

        private readonly Action processCompletedDelegate;

        internal CommandLineTools(Action<string> processUpdateDelegate, Action processCompletedDelegate)
        {
            this.processUpdateDelegate = processUpdateDelegate;
            this.processCompletedDelegate = processCompletedDelegate;
        }

        internal void RunCommandLineApp(string command, string arguments)
        {
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;

            // Start a process with the filename or path with filename e.g. "cmd". Please note the 
            //using statemant
            p.StartInfo =
                new ProcessStartInfo(
                    command,
                    arguments);

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;

            // Allows to raise events
            p.EnableRaisingEvents = true;
            p.StartInfo.CreateNoWindow = true;
            // Eventhander for data
            p.OutputDataReceived += OnDataReceived;
            // Eventhandler for error
            p.ErrorDataReceived += OnErrorDataReceived;
            // Eventhandler wich fires when exited
            p.Exited += OnExited;
            // Starts the process
            p.Start();

            p.BeginOutputReadLine();
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.processUpdateDelegate?.Invoke($"{Environment.NewLine}{e.Data}");
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.processUpdateDelegate?.Invoke($"{Environment.NewLine}{e.Data}");
        }

        private void OnExited(object sender, System.EventArgs e)
        {
            this.processCompletedDelegate?.Invoke();
        }
    }
}
