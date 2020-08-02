namespace spotify
{
    using System;
    using Google.Cloud.Firestore;

    [FirestoreData]
    public class TokenData
    {
        [FirestoreProperty]
        public string access_token { get; set; }
        [FirestoreProperty]
        public string refresh_token { get; set; }
        [FirestoreProperty]
        public string scope { get; set; }
        [FirestoreProperty]
        public string token_type { get; set; }
        [FirestoreProperty]
        public int expires_in { get; set; }

        [FirestoreDocumentCreateTimestamp]
        public Timestamp CreatedTime { get; set; }
        public bool IsExpired() => 
            expires_in - Math.Abs((DateTimeOffset.UtcNow - CreatedTime.ToDateTimeOffset()).TotalSeconds) < 0;
    }
}