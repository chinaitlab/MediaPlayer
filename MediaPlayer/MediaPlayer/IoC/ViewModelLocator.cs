using MediaPlayer.ViewModel;

namespace MediaPlayer.IoC
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => IocKernel.Get<MainViewModel>();
        public HomeViewModel HomeViewModel => IocKernel.Get<HomeViewModel>();
    }
}