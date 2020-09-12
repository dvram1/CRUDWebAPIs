using NUnit.Framework;
using System;
using System.Collections.Generic;
using ProjectCRUD.Models;
using ProjectCRUD.Services;
using ProjectCRUD.Controllers;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace ProjCRUD.Tests
{
    [TestFixture]
    public class IntegrationTests_StudentController
    {
        StudentService _studentService;
        StudentsController _studentsController;
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
            _studentsController = new StudentsController(_studentService);
            _studentsController.Request = new HttpRequestMessage();
            _studentsController.Configuration = new HttpConfiguration();
        }

       
        [Test]
        public void TestGetAll_Success()
        {
            // Arrange
            CreateTestData();
 
            //Act
            HttpResponseMessage results = _studentsController.GetAll();

            //Assert
            Assert.IsTrue(results.StatusCode == HttpStatusCode.OK);
            List<Student> resultList;
            Assert.IsTrue(results.TryGetContentValue<List<Student>>(out resultList));
            Assert.AreEqual(5, resultList.Count);
            Assert.True(CompareLists(resultList, _inputlist));

            //Cleanup
            ClearTestData();
        }

        [Test]
        public void TestGetDeptWiseCount_Success()
        {
            // Arrange
            CreateTestData();

            //Act
            HttpResponseMessage results = _studentsController.GetDeptWiseCount();

            //Assert
            List<DepartmentCount> deptList = new List<DepartmentCount>()
            {
                new DepartmentCount { department = "Arts", count = 1},
                new DepartmentCount { department = "Engineering", count = 2},
                new DepartmentCount { department = "Mathematics", count = 1},
                new DepartmentCount { department = "Science", count = 1}
            };
            Assert.IsTrue(results.StatusCode == HttpStatusCode.OK);
           
            //Cleanup
            ClearTestData();
        }

        [Test]
        public void TestGet_Success()
        {
            // Arrange
            CreateTestData();

            //Act
            HttpResponseMessage results = _studentsController.Get(_inputlist[0].studentID);

            //Assert
            Assert.IsTrue(results.StatusCode == HttpStatusCode.OK);
            Student result;
            Assert.IsTrue(results.TryGetContentValue<Student>(out result));
            Assert.True(CompareStudent(result, _inputlist[0]));

            //Cleanup
            ClearTestData();
        }

        [Test]
        public void TestGet_Failure_ReturnsNotFound()
        {
            // Arrange
            CreateTestData();

            //Act
            HttpResponseMessage results = _studentsController.Get("675432");

            //Assert
            Assert.IsTrue(results.StatusCode == HttpStatusCode.NotFound);
           
            //Cleanup
            ClearTestData();
        }

        [Test]
        public void TestCreate_Success()
        {
            string message;
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            HttpResponseMessage results = _studentsController.Post(s2);
            Assert.IsTrue(results.StatusCode == HttpStatusCode.Created);
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));
            Assert.True(_studentService.Delete(s2.studentID, out message));
        }

        [Test]
        public void TestCreate_Failure_ReturnsBadRequest()
        {
            
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            HttpResponseMessage results = _studentsController.Post(s2);

            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));

            //Try to create the same student document again
            string message = "student already exists";
            results = _studentsController.Post(s2);
            Assert.IsTrue(results.StatusCode == HttpStatusCode.BadRequest);
            Object responseContent;
            Assert.IsTrue(results.TryGetContentValue(out responseContent));
            HttpError error = responseContent as HttpError;
            if (error != null)
            {
                string errorMessage = error.Message;
                string errorMessageDetail = error.MessageDetail;

            }
            Assert.IsTrue(error.Message == message);


            Assert.True(_studentService.Delete(s2.studentID, out message));
        }

        [Test]
        public void TestDelete_Success()
        {
            string message;
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            HttpResponseMessage results = _studentsController.Post(s2);
            Assert.IsTrue(results.StatusCode == HttpStatusCode.Created);
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));
            Assert.True(_studentService.Delete(s2.studentID, out message));
            listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 0);
        }

        [Test]
        public void TestDelete_Failure_ReturnsBadRequest()
        {

            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            HttpResponseMessage results = _studentsController.Post(s2);

            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));

            //Try to create the same student document again
            string message = "Student not found to Delete";
            results = _studentsController.Delete("123456");
            Assert.IsTrue(results.StatusCode == HttpStatusCode.BadRequest);
            Object responseContent;
            Assert.IsTrue(results.TryGetContentValue(out responseContent));
            HttpError error = responseContent as HttpError;
            if (error != null)
            {
                string errorMessage = error.Message;
                string errorMessageDetail = error.MessageDetail;

            }
            Assert.IsTrue(error.Message == message);

            //Cleanup
            Assert.IsTrue(_studentService.Delete(s2.studentID, out message));
        }

        [Test]
        public void TestUpdate_Success()
        {
            string message;
            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            HttpResponseMessage results = _studentsController.Post(s2);
            Assert.IsTrue(results.StatusCode == HttpStatusCode.Created);
            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));

            //Modify the student information and send the Update call to COntroller
            s2.grade = "B+";
             results = _studentsController.Put(s2);

            Assert.IsTrue(results.StatusCode == HttpStatusCode.OK);
            listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));

            Assert.True(_studentService.Delete(s2.studentID, out message));
            listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 0);
        }

        [Test]
        public void TestUpdate_Failure_ReturnsBadRequest()
        {

            Student s2 =
                new Student { Id = "", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science" };
            HttpResponseMessage results = _studentsController.Post(s2);

            List<Student> listStudents = _studentService.GetAll();
            Assert.True(listStudents.Count == 1);
            Assert.True(CompareStudent(listStudents[0], s2));

            //Try to Update a non-existing document
            string message = "student not found to update";
            s2.studentID = "123456";
            results = _studentsController.Put(s2);
            Assert.IsTrue(results.StatusCode == HttpStatusCode.BadRequest);
            Object responseContent;
            Assert.IsTrue(results.TryGetContentValue(out responseContent));
            HttpError error = responseContent as HttpError;
            if (error != null)
            {
                string errorMessage = error.Message;
                string errorMessageDetail = error.MessageDetail;

            }
            Assert.IsTrue(error.Message == message);
            
            //Cleanup
            s2.studentID = "222222";
            Assert.True(_studentService.Delete(s2.studentID, out message));
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
            for (int i = 0; i < list1.Count; i++)
            {
                if (!CompareStudent(list1[i], list2[i]))
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
