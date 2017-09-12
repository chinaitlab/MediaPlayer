using System;
using System.Reflection;
using System.Windows;
using MediaPlayer.Core.Logger;
using MediaPlayer.IoC;

namespace MediaPlayer
{
    public partial class App
    {
        public static Version CurrentVersion;
        protected override void OnStartup(StartupEventArgs e)
        {
            IocKernel.Initialize(new IocConfiguration());
            LogVersions();
            base.OnStartup(e);
        }

        private static void LogVersions()
        {
            CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;
            Constants.Loggers.Info(CurrentVersion);
        }
    }
}