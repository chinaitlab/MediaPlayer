using System.Collections.Generic;
using MediaPlayer.ViewModel;
using NLog;

namespace MediaPlayer.IoC
{
    public class Constants
    {
        public static IList<ILogger> Loggers => IocKernel.Get<IList<ILogger>>();

        public static IList<BaseViewModel> Modules => IocKernel.Get<IList<BaseViewModel>>();
    }
}