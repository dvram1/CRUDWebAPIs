using System;
using NUnit.Framework;
using ProjectCRUD.Models;
using ProjectCRUD.Services;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Linq;

namespace ProjCRUD.Tests
{

    [TestFixture]
    public class IntegrationTests_StudentService
    {
        StudentService _studentService;
        List<Student> _inputlist;
        [SetUp]
        public void Setup()
        {
            //Database related
            var connectionString = ConfigurationManager.AppSettings["TestMongoDBConectionString"];
            var client = new MongoClient(connectionString);
            var databaseName = ConfigurationManager.AppSettings["TestMongoDBDatabaseName"];
            var collectionName = ConfigurationManager.AppSettings["TestMongoDBCollectionName"];
            var database = client.GetDatabase(databaseName);
            var IMongoCol = database.GetCollection<Student>(collectionName);
            _studentService = new StudentService(IMongoCol);
        }

        [TearDown]
        public void Teardown()
        {
            _studentService = null;
        }
        [Test]
        public void TestGetAll()
        {
            CreateTestData();

            List<Student> listStudents = _studentService.GetAll();

            Assert.True(CompareLists(listStudents, _inputlist));
            //Cleanup
            ClearTestData();
        }

        [Test]
        public void TestGet()
        {
            CreateTestData();

            Student st = _studentService.Get(_inputlist[0].studentID);

            Assert.True(CompareStudent(st,_inputlist[0]));
            //Cleanup
            ClearTestData();
        }

        [Test]
        public void TestCreate()
        {
            string message;
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            Assert.True(_studentService.Create(s2, out message));

            Assert.True(message == null);
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));
            Assert.True(_studentService.Delete(s2.studentID, out message));
        }

        [Test]
        public void TestUpdate()
        {
            string message;
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            Assert.True(_studentService.Create(s2, out message));
            Assert.True(message == null);
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));
        
            //Test the Update function here
            s2.grade = "B+";
            Assert.True(_studentService.Update(s2, out message));

            //Verify
            Student updatedStudent = _studentService.Get(s2.studentID, out message);
            Assert.True(CompareStudent(updatedStudent, s2));
            Assert.True(_studentService.Delete(s2.studentID, out message));
        }

        [Test]
        public void TestDelete()
        {
            string message;
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            Assert.True(_studentService.Create(s2, out message));
            Assert.True(message == null);
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));

            //Test the Delete function here
            Assert.True(_studentService.Delete(s2.studentID, out message));
            Assert.True(message == null);

            //Verify
            listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 0);

        }

        public void CreateTestData()
        {
           
             _inputlist = new List<Student>()
            {
                new Student {Id = "", studentID = "111111", firstName = "Tom", lastName = "Cruise", grade = "A+", department = "Engineering"},
                new Student {Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science"},
                new Student {Id = "", studentID = "333333", firstName = "Jason", lastName = "Bourne", grade = "B", department = "Arts"},
                new Student {Id = "", studentID = "444444", firstName = "Bruce", lastName = "Willis", grade = "C", department = "Mathematics"},
                new Student {Id = "", studentID = "555555", firstName = "Meg", lastName = "Ryan", grade = "B+", department = "Engineering"},
            };

            // populate test database with a few studet records
            string message;
            for (int i = 0; i < _inputlist.Count; i++)
                Assert.True(_studentService.Create(_inputlist[i], out message));
        }

        public void ClearTestData()
        {
            //Cleanup
            //Remove all the student objects added earlier 
            string message;
            for (int i = 0; i < _inputlist.Count; i++)
                Assert.True(_studentService.Delete(_inputlist[i].studentID, out message));

            //make sure database is empty
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 0);
        }

        //We need this function since Studet also  contains id field that is auto-generated
        // and we cannot predict that
        public bool CompareLists(List<Student> list1, List<Student> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            for(int i=0; i<list1.Count; i++)
            {
                if(!CompareStudent(list1[i], list2[i]) )
                     return false;
            }
            return true;
        }

        public bool CompareStudent(Student s1, Student s2)
        {
            if (s1.studentID.CompareTo(s2.studentID) != 0
                || s1.firstName.CompareTo(s2.firstName) != 0
                || s1.lastName.CompareTo(s2.lastName) != 0
                || s1.grade.CompareTo(s2.grade) != 0
                || s1.department.CompareTo(s2.department) != 0)
                return false;
            return true;
        }

    }

}
