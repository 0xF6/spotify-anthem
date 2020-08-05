namespace spotify.services
{
    using Google.Cloud.Firestore;
    using Microsoft.Extensions.Configuration;

    public class FirestoreService
    {
        private FirestoreDb db { get; }

        public FirestoreService(IConfiguration config) => 
            db = FirestoreDb.Create(config["db_id"]);

        public CollectionReference Cache => 
            db.Collection("spotify")
                .Document("data")
                .Collection("cache");
        public CollectionReference Settings => 
            db.Collection("spotify")
            .Document("data")
            .Collection("settings");
    }
}