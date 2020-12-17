using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Markup;
using BlitFlashNet.GitHubApi;
using BlitFlashNet.ViewModels;

namespace BlitFlashNet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var w = new MainWindow();
            w.DataContext = new AppViewModel();
            w.Show();
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
    }
}
