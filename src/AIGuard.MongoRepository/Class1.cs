using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AIGuard.MongoRepository
{
    public class BlueIris
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
    }
}
