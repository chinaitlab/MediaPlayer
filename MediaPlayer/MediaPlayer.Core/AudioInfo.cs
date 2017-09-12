namespace MediaPlayer.Core
{
    public class AudioInfo
    {
        public Status Status { get; set; }
        public Metadata Metadata { get; set; }
        public int ResultType { get; set; }
    }

    public class Status
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public string Version { get; set; }
    }

    public class Metadata
    {
        public Music[] Music { get; set; }
        public string TimestampUtc { get; set; }
    }

    public class Music
    {
        public ExternalIds ExternalIds { get; set; }
        public int PlayOffsetMs { get; set; }
        public string ReleaseDate { get; set; }
        public Artist[] Artists { get; set; }
        public ExternalMetadata ExternalMetadata { get; set; }
        public string Title { get; set; }
        public int DurationMs { get; set; }
        public Album Album { get; set; }
        public string Acrid { get; set; }
        public int ResultFrom { get; set; }
        public int Score { get; set; }
    }

    public class ExternalIds
    {
    }

    public class ExternalMetadata
    {
    }

    public class Album
    {
        public string Name { get; set; }
    }

    public class Artist
    {
        public string Name { get; set; }
    }
}