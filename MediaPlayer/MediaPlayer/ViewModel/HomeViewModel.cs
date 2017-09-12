using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MediaPlayer.Core;
using MediaPlayer.Core.Additional;
using MediaPlayer.Model;
using MediaPlayer.Tools;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace MediaPlayer.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private MusicModel _currentMusic;

        public override string MenuCaption
        {
            get => _menuCaption;
            protected set
            {
                OnPropertyChanging();
                _menuCaption = value; 
                OnPropertyChanged();
            }
        }

        public override PackIconKind MenuIcon { get; } = PackIconKind.Home;
        public override Brush IconColor { get; } = Brushes.LightBlue;

        private readonly List<string> _files = new List<string> { "test1.mp3", "test2.mp3", "test3.mp3", "test4.mp3" };
        private readonly List<MusicModel> _musics = new List<MusicModel>();
        private int _current;
        private string _menuCaption;

        public MusicModel CurrentMusic
        {
            get => _currentMusic;
            set
            {
                OnPropertyChanging();
                _currentMusic = value;
                OnPropertyChanged();
            }
        }

        public HomeViewModel()
        {
            NextMusic = new RelayCommand(x => { MoveToNext(); }, x => true);
            PrevMusic = new RelayCommand(x => { MoveToPrev(); }, x => true);

            foreach (var file in _files)
            {
                var music = Recognizer.GetInfo(file).Metadata.Music.FirstOrDefault();
                if (music == null) return;
                var name = $"{string.Join(", ", music.Artists.Select(x => x.Name))} - {music.Title}";
                var image = ImageLoader.GetFirstImage(name);
                var clr = DominantHueColorCalculator.CalculateDominantColor(image);
                var clr2 = DominantHueColorCalculator.GetDarkenColor(clr);
                var m = new MusicModel(music, image, clr, clr2);
                _musics.Add(m);
            }

            SetCurrentMusic();
        }

        public ICommand NextMusic { get; set; }
        public ICommand PrevMusic { get; set; }

        private void MoveToNext()
        {
            _current++;
            if (_musics.Count == _current)
                _current = 0;
            SetCurrentMusic();
        }

        private void MoveToPrev()
        {
            _current--;
            if (-1 == _current)
                _current = _musics.Count - 1;
            SetCurrentMusic();
        }

        private void SetCurrentMusic()
        {
            CurrentMusic = _musics[_current];
            MenuCaption = $"{string.Join(", ", CurrentMusic.Metadata.Artists.Select(x => x.Name))} - {CurrentMusic.Metadata.Title}";
        }
    }
}