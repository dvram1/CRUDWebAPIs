using MongoDB.Driver;
using ProjectCRUD.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectCRUD.Services
{
    public class StudentService : IStudentService
    {
        private readonly IMongoCollection<Student> _students;

        public StudentService()
        {
            var connectionString = ConfigurationManager.AppSettings["MongoDBConectionString"];

            var client = new MongoClient(connectionString);
            var databaseName = ConfigurationManager.AppSettings["MongoDBDatabaseName"];
            var collectionName = ConfigurationManager.AppSettings["MongoDBCollectionName"];
            var database = client.GetDatabase(databaseName);
            _students = database.GetCollection<Student>(collectionName);

        }
        //  returns a student document given a studentID
        public Student Get(string id)
        {
             return _students.Find<Student>(student => student.studentID == id).FirstOrDefault();
        }

        // Returns all student documents existing in collection
        public List<Student> GetAll()
        {
            return _students.Find(student => true).ToList();
        }

        //returns each department name and count of students under each department
        public IEnumerable<object> GetDeptWiseCount()
        {
            var deptWiseCount = _students.AsQueryable()
                                .GroupBy(s=> new { s.department})
                                .Select(n => new
                                {
                                    department = n.Key.department,
                                    count = n.Count()
                                });

            return  deptWiseCount.ToList(); 
        }

        //Deletes a student document from collection if there is a Student document present in database with the StudentID passed
        public void Delete(string id)
        {
            _students.DeleteOne(student => student.studentID == id);
        }

        //creates a student document in the appropriate collection if it does not exist
        public void Create(Student student)
        {
            _students.InsertOne(student);
        }

        // Updates a student document  if the Student document is present in database 
        public void Update(Student student)
        {
            var st = Get(student.studentID);
            student.Id = st.Id; // the ID of the student must be copied into the doc that overwrites 
            _students.ReplaceOne(s => s.studentID == student.studentID, student);
        }

    }
}