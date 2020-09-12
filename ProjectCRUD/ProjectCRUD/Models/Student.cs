using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


namespace ProjectCRUD.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        //student ID could also have an alpha numeric character in the future and hence defined it as a string instead of an integer
        public string studentID { get; set; }
        public string grade { get; set; }
        public string department { get; set; }
    }
}