using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MediaPlayer.Core.Properties;

namespace MediaPlayer.ViewModel
{
    public abstract class BaseViewModel : DependencyObject, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public abstract string MenuCaption { get; protected set; }
        public abstract PackIconKind MenuIcon { get; }
        public abstract Brush IconColor { get; }

        public virtual string Caption => $"{MenuCaption} (v. {App.CurrentVersion})";

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}