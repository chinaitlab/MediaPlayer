using System;
using System.Linq;
using MediaPlayer.Core;

namespace MediaPlayer.Tests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var music = Recognizer.GetInfo("test.mp3").Metadata.Music.FirstOrDefault();
            string result = null;
            if (music != null) result = $"{string.Join(", ", music.Artists.Select(x => x.Name))} - {music.Title}";
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}