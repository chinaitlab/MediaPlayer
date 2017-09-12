using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MediaPlayer.IoC;

namespace MediaPlayer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public override string MenuCaption { get; protected set; } = "Template";
        public override PackIconKind MenuIcon { get; } = PackIconKind.BorderNone;
        public override Brush IconColor { get; } = Brushes.Black;

        private ObservableCollection<BaseViewModel> _menuItems;
        private BaseViewModel _selectedMenuItem;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<BaseViewModel>(Constants.Modules);
            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        public BaseViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                OnPropertyChanging();
                _selectedMenuItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BaseViewModel> MenuItems
        {
            get => _menuItems;
            set
            {
                OnPropertyChanging();
                _menuItems = value;
                OnPropertyChanged();
            }
        }
    }
}