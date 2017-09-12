using System.Collections.Generic;
using MediaPlayer.ViewModel;
using Ninject;
using Ninject.Modules;

namespace MediaPlayer.IoC
{
    public static class IocKernel
    {
        private static StandardKernel _kernel;

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static void Initialize(params INinjectModule[] modules)
        {
            if (_kernel != null) return;
            _kernel = new StandardKernel(modules);
            var views = new List<BaseViewModel> { Get<HomeViewModel>() };
            _kernel.Bind<IList<BaseViewModel>>().ToConstant(views);
        }
    }
}