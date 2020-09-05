using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


namespace ProjectCRUD.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string studentID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string grade { get; set; }
        public string department { get; set; }
    }
}