using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaPlayer.Core;
using MediaPlayer.Core.Additional;
using MediaPlayer.Core.Model;
using MediaPlayer.Tools;

namespace MediaPlayer.Model
{
    public class MusicModel : BaseModel
    {
        private Brush _mainBrush;
        private Brush _accentBrush;
        private BitmapImage _image;
        private Music _metadata;

        public Brush MainBrush
        {
            get => _mainBrush;
            set
            {
                OnPropertyChanging();
                _mainBrush = value;
                OnPropertyChanged();
            }
        }

        public Brush AccentBrush
        {
            get => _accentBrush;
            set
            {
                OnPropertyChanging();
                _accentBrush = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Image
        {
            get => _image;
            set
            {
                OnPropertyChanging();
                _image = value;
                OnPropertyChanged();
            }
        }

        public Music Metadata
        {
            get => _metadata;
            set
            {
                OnPropertyChanging();
                _metadata = value;
                OnPropertyChanged();
            }
        }

        public MusicModel(Music metadata, byte[] image, Color mainColor, Color accentColor)
        {
            Metadata = metadata;
            MainBrush = new SolidColorBrush(mainColor);
            AccentBrush = new SolidColorBrush(accentColor);
            Image = image.LoadImage();
            DominantHueColorCalculator.CalculateDominantColor(image);
        }
    }
}