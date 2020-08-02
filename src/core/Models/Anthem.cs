namespace spotify
{
    using System;
    using System.Linq;
    using Google.Cloud.Firestore;
    using SpotifyAPI.Web;

    [FirestoreData]
    public class Anthem
    {
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string ArtistName { get; set; }
        [FirestoreProperty]
        public string Uri { get; set; }
        [FirestoreProperty]
        public string PreviewUrl { get; set; }
        [FirestoreProperty]
        public string ImageUrl { get; set; }

        [FirestoreDocumentUpdateTimestamp]
        public Timestamp UpdateTime { get; set; }

        // current anthem expired after 6 hours
        public bool IsExpired() => 
            TimeSpan.FromHours(6).TotalSeconds - Math.Abs((DateTimeOffset.UtcNow - UpdateTime.ToDateTimeOffset()).TotalSeconds) < 0;

        public static Anthem From(FullTrack track) =>
            new Anthem
            {
                Name = track.Name,
                Uri = track.Uri,
                ArtistName = track.Artists
                    .Select(x => x.Name)
                    .Aggregate((x, y) => $"{x} & {y}"),
                PreviewUrl = track.PreviewUrl,
                ImageUrl = track.Album.Images.First().Url
            };
    }
}