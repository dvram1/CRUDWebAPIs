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

        public StudentsController(IStudentService studentSvc)
        {
            _studentService = studentSvc;
        }

        //GET -  returns a student document given a studentID
        [Route("api/students/Get/{id}")]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                string message = null;
                var student = _studentService.Get(id,out message);
                if(message!=null) //if validation errors on id, return error
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
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
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No students found");
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
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Department Info found");
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
                string message= null;
                if(_studentService.Create(student, out message))
                    return Request.CreateResponse(HttpStatusCode.Created, student, Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
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
                string message = null;
                if (_studentService.Delete(id, out message))
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
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
                string message = null;
                if (_studentService.Update(student, out message))
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in execution of API", ex);
            }

        }
    }
}