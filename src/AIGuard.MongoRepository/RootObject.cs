using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.MongoRepository
{
    public class DetectionRoot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Topic { get; set; }
        public Prediction Payload { get; set; }
        public int Qos { get; set; }
        public bool Retain { get; set; }
        public string Msgid { get; set; }
        public DateTime Date { get; set; }
    }
}
