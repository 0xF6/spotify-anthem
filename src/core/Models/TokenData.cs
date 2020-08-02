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

        [FirestoreDocumentUpdateTimestamp]
        public Timestamp UpdateTime { get; set; }
        public bool IsExpired() => 
            expires_in - Math.Abs((DateTimeOffset.UtcNow - UpdateTime.ToDateTimeOffset()).TotalSeconds) < 0;
    }
}