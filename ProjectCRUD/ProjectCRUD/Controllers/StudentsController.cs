using System.Linq;
using System.Web.Http;
using ProjectCRUD.Models;
using ProjectCRUD.Services;
using System.Net.Http;
using System.Net;
using System;

namespace ProjectCRUD.Controllers
{
    public class StudentsController : ApiController
    {
        private readonly IStudentService _studentService;

        public StudentsController()
        {
            _studentService = new StudentService();
        }

        //GET -  returns a student document given a studentID
        [Route("api/students/Get/{id}")]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                var student = _studentService.Get(id);
                if (student != null)
                    return Request.CreateResponse(HttpStatusCode.OK, student, Configuration.Formatters.JsonFormatter);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found with given id");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }

        }

        //GET -  Returns all student documents existing in collection
        [HttpGet]
        [Route("api/students/GetAll")]
        public HttpResponseMessage GetAll()
        {
            try
            {
                var students = _studentService.GetAll();
                if (students.Any())
                    return Request.CreateResponse(HttpStatusCode.OK, students, Configuration.Formatters.JsonFormatter);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No students found.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }
        }

        //GET - returns each department name and count of students under each department
        [HttpGet]
        [Route("api/students/GetDeptCount")]
        public HttpResponseMessage GetDeptWiseCount()
        {
            try
            {
                var deptsInfo = _studentService.GetDeptWiseCount();
                if (deptsInfo.Any())
                    return Request.CreateResponse(HttpStatusCode.OK, deptsInfo, Configuration.Formatters.JsonFormatter);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Departent Info found.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }
        }

        // POST -  Create method - creates a student document in the appropriate collection if it does not exist
        [HttpPost]
        [Route("api/students/Create")]
        public HttpResponseMessage Post([FromBody]Student student)
        {
            try
            {
                if (student != null)
                {
                    var s = _studentService.Get(student.studentID);
                    if (s == null) // This student does not exist in the collection and hence we can add to collection
                        _studentService.Create(student);
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "student already exists");

                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "student parameter is null.");

                return Request.CreateResponse(HttpStatusCode.Created, student, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }
        }

        // DELETE - Deletes a student document from collection if there is a Student document present in database with the StudentID passed
        [HttpDelete]
        [Route("api/students/Delete/{id}")]
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                var s = _studentService.Get(id);

                if (s == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found to Delete ");
                }
                _studentService.Delete(id);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }

        }

        // PUT - Update method - Updates a student document  if the Student document is present in database 
        [HttpPut]
        [Route("api/students/Update")]
        public  HttpResponseMessage Put([FromBody]Student student)
        {
            try
            {
                 var s = _studentService.Get(student.studentID);
                if (s == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found to update ");
                }
                _studentService.Update(student);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }

        }
    }
}