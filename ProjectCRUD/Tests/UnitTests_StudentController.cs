using System;
using NUnit.Framework;
using ProjectCRUD.Models;
using ProjectCRUD.Services;
using ProjectCRUD.Controllers;
using Moq;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace ProjCRUD.Tests
{
    public class DepartmentCount
    {
        public string department { get; set; }
        public int count { get; set; }
    }

    
    [TestFixture]
    public class UnitTests_StudentController
    {
        Mock<IStudentService> _mockStudentService;
        List<Student> _list;
        delegate bool MockOutDelegate(Student s, out string message);
        delegate bool MockOutDeleteDelegate(string id, out string message);
        [SetUp]
        public void Setup()
        {
            _mockStudentService = new Mock<IStudentService>();
           
           
        }

        [Test]
        public void GetAll_SuccessTestCase()
        {
            // Arrange
            _list = new List<Student>()
            {
                new Student {Id = "f12345672", studentID = "111111", firstName = "Tom", lastName = "Cruise", grade = "A+", department = "Engineering"},
                new Student {Id = "f12345673", studentID = "222222", firstName = "Jack", lastName = "Ryan", grade = "A", department = "Science"},
                new Student {Id = "f12345674", studentID = "333333", firstName = "Jason", lastName = "Bourne", grade = "B", department = "Arts"},
                new Student {Id = "f12345675", studentID = "444444", firstName = "Bruce", lastName = "Willis", grade = "C", department = "Mathematics"},
                new Student {Id = "f12345676", studentID = "555555", firstName = "Meg", lastName = "Ryan", grade = "B+", department = "Engineering"},
            };
            _mockStudentService.Setup(c => c.GetAll()).Returns(_list);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();
           
            //Act
            HttpResponseMessage results = studentsController.GetAll();

            //Assert
            List<Student> resultList;
            Assert.IsTrue(results.TryGetContentValue<List<Student>>(out resultList));
            Assert.AreEqual(5, resultList.Count);
            Assert.AreEqual( resultList, _list);

        }

        [Test]
        public void GetAll_FailureTestCase()
        {
            // Arrange
            _mockStudentService.Setup(c => c.GetAll()).Returns(new List<Student>());

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.GetAll();

            //Assert
            string message = "No students found";
            Assert.IsTrue(results.StatusCode == HttpStatusCode.NotFound);
            Object responseContent;
            Assert.IsTrue(results.TryGetContentValue(out responseContent));
            HttpError error = responseContent as HttpError;
            if (error != null)
            {
                string errorMessage = error.Message;
                string errorMessageDetail = error.MessageDetail;

            }
            Assert.IsTrue(error.Message == message);

        }

        [Test]
        public void GetAll_FailureTestCase_ThrowsException()
        {
            // Arrange
            _mockStudentService.Setup(c => c.GetAll()).Throws(new Exception());

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.GetAll();

            //Assert
            string message = "Error in execution of API";
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

        }

        [Test]
        public void Get_SuccessTestCase()
        {
            // Arrange
            Student student = new Student { Id = "f12345672", studentID = "111111", firstName = "Tom", lastName = "Cruise", grade = "A+", department = "Engineering" };
            string message;
            _mockStudentService.Setup(c => c.Get("111111",out message)).Returns(student);
            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Get("111111");

            //Assert
            Student result;
            Assert.IsTrue(results.TryGetContentValue<Student>(out result));
            Assert.AreEqual(result, student);

        }

        [Test]
        public void Get_FailureTestCase()
        {
            // Arrange
            Student student = new Student { Id = "f12345672", studentID = "111111", firstName = "Tom", lastName = "Cruise", grade = "A+", department = "Engineering" };
            string message = "studentID must be a 6 digit number";
            _mockStudentService.Setup(c => c.Get("111111", out message)).Returns(student);
            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Get("111111");

            //Assert
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

        }

        [Test]
        public void Get_FailureTestCase_ThrowsException()
        {
            // Arrange
            Student student = new Student { Id = "f12345672", studentID = "111111", firstName = "Tom", lastName = "Cruise", grade = "A+", department = "Engineering" };
            string message = "studentID must be a 6 digit number";
            _mockStudentService.Setup(c => c.Get("111111", out message)).Throws(new Exception());
            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Get("111111");

            //Assert
            message = "Error in execution of API";
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

        }

        [Test]
        public void GetDeptWiseCount_SuccessTestCase()
        {
            // Arrange
            List<DepartmentCount> deptList = new List<DepartmentCount>()
            {
                new DepartmentCount { department = "Engineering", count = 3},
                new DepartmentCount { department = "Science", count = 2},
                new DepartmentCount { department = "Mathematics", count = 3},
                new DepartmentCount { department = "Arts", count = 1}
            };
           
            _mockStudentService.Setup(c => c.GetDeptWiseCount()).Returns(deptList);
            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.GetDeptWiseCount();

            //Assert
            List<DepartmentCount> result;
            Assert.IsTrue(results.TryGetContentValue<List<DepartmentCount>>(out result));
            Assert.AreEqual(result, deptList);

        }

        [Test]
        public void GetDeptWiseCount_FailureTestCase()
        {
            // Arrange
            
            _mockStudentService.Setup(c => c.GetDeptWiseCount()).Returns(new List<Student>());
            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.GetDeptWiseCount();

            //Assert
            string message = "No Department Info found";
            Assert.IsTrue(results.StatusCode == HttpStatusCode.NotFound);
            Object responseContent;
            Assert.IsTrue(results.TryGetContentValue(out responseContent));
            HttpError error = responseContent as HttpError;
            if (error != null)
            {
                string errorMessage = error.Message;
                string errorMessageDetail = error.MessageDetail;

            }
            Assert.IsTrue(error.Message == message);

        }
        [Test]
        public void GetDeptWiseCount_FailureTestCase_ThrowsException()
        {
            // Arrange

            _mockStudentService.Setup(c => c.GetDeptWiseCount()).Throws(new Exception());
            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.GetDeptWiseCount();

            //Assert
            string message = "Error in execution of API";
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

        }


        //test for Create Student  call
        [Test]
        public void Post_SuccessfulTestCase()
        {
            // Arrange

            Student student=null;
            string message=null;
            _mockStudentService.Setup(c => c.Create(student,out message)).Returns(true);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Post(student);

            //Assert
            Assert.IsTrue(results.StatusCode == HttpStatusCode.Created);
        }

        //test for Create Student  call
        [Test]
        public void Post_FailureTestCase()
        {
            // Arrange

            Student student = null;
            string message = "student already exists";
            _mockStudentService.Setup(c => c.Create(student, out message)).Returns(false);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Post(student);

            //Assert
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
        }

        [Test]
        public void Post_FailureTestCase_ThrowsException()
        {
            // Arrange

            Student student = null;
            string message = "student already exists";
            _mockStudentService.Setup(c => c.Create(student, out message)).Throws(new Exception());

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Post(student);

            //Assert
            message = "Error in execution of API";
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
        }

        [Test]
        public void Update_SuccessfulTestCase()
        {
            // Arrange

            Student student = null;
            string message = null;
            _mockStudentService.Setup(c => c.Update(student, out message)).Returns(true);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Put(student);

            //Assert
            Assert.IsTrue(results.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public void Update_FailureTestCase()
        {
            // Arrange

            Student student = null;
            string message = "student not found to update";
            _mockStudentService.Setup(c => c.Update(student, out message)).Returns(false);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Put(student);

            //Assert
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
        }

        [Test]
        public void Update_FailureTestCase_ThrowsException()
        {
            // Arrange

            Student student = null;
            string message = "student not found to update";
            _mockStudentService.Setup(c => c.Update(student, out message)).Throws(new Exception());

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Put(student);

            //Assert
            message = "Error in execution of API";
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
        }

        [Test]
        public void Delete_SuccessfulTestCase()
        {
            // Arrange

            string message = null;
            _mockStudentService.Setup(c => c.Delete(It.IsAny<string>(), out message)).Returns(true);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Delete(It.IsAny<string>());

            //Assert
            Assert.IsTrue(results.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public void Delete_FailureTestCase()
        {
            // Arrange

            string message = "student not found to Delete";
            _mockStudentService.Setup(c => c.Delete(It.IsAny<string>(), out message)).Returns(false);

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Delete(It.IsAny<string>());

            //Assert
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
        }

        [Test]
        public void Delete_FailureTestCase_ThrowsException()
        {
            // Arrange

            string message = "student not found to Delete";
            _mockStudentService.Setup(c => c.Delete(It.IsAny<string>(), out message)).Throws(new Exception());

            StudentsController studentsController = new StudentsController(_mockStudentService.Object);
            studentsController.Request = new HttpRequestMessage();
            studentsController.Configuration = new HttpConfiguration();

            //Act
            HttpResponseMessage results = studentsController.Delete(It.IsAny<string>());

            //Assert
            message = "Error in execution of API";
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
        }
    }
}
