using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlitFlashNet.Util
{
    internal static class FileDownLoader
    {
        internal static void DownloadFile(string fileToDownload, string downloadPath, Action<object, DownloadProgressChangedEventArgs> downloadProgressDelegate, Action<object, AsyncCompletedEventArgs> downloadCompletedDelegate)
        {
            Thread thread = new Thread(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgressDelegate);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompletedDelegate);
                client.DownloadFileAsync(new Uri(fileToDownload), downloadPath);
            });
            thread.Start();
        }
    }
}
