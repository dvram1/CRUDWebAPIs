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
        private const int studentIDLength = 6;
        private const int maxGradeLength = 2;
        private readonly string[] departments = new string[] { "Mathematics", "Science", "Engineering", "Arts", "Business" };
        private readonly string[] grades = new string[] { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "D-", "F" };

        private readonly IMongoCollection<Student> _students;

        public StudentService(IMongoCollection<Student> studentsCollection)
        {
            _students = studentsCollection;

        }
        //  returns a student document given a studentID
        public Student Get(string id, out string message)
        {
            if (!ValidateStudentID(id))
            {
                message = "studentID must be a 6 digit number";
                return null;
            }
            message = null;
            return _students.Find<Student>(student => student.studentID == id).FirstOrDefault();
        }

        //  returns a student document given a studentID. This is typically called if the validation check for id is already done.
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
        public bool Delete(string id, out string message)
        {
            if (!ValidateStudentID(id))
            {
                message = "studentID must be a 6 digit number";
                return false;
            }
            var s = Get(id); 

            if (s != null) // if the student exists, we can delete the corresponding document
            {
                _students.DeleteOne(student => student.studentID == id);
                message = null;
                return true;
            }

            message = "Student not found to Delete";
            return false;
        }

        //creates a student document in the appropriate collection if it does not exist
        public bool Create(Student student, out string message)
        {
            if (student == null)
            {
                message = "student is null";
                return false;
            }

            if (!ValidateStudent(student, out message))
                return false;

            var s = Get(student.studentID);
            if (s == null) // This student does not exist in the collection and hence we can add to collection
             {
                _students.InsertOne(student);
                 message = null;
                 return true; // success
             }
             else
               message = "student already exists";

            return false; // return failure
        }

         // Updates a student document  if the Student document is present in database 
        public bool Update(Student student, out string message)
        {
            if (student == null)
            {
                message = "student is null";
                return false;
            }

            if (!ValidateStudent(student, out message))
                return false;

            var s = Get(student.studentID);
            if (s != null) // This student exists in the collection and hence we can update the student document
            {
                student.Id = s.Id; // the ID of the student must be copied into the doc that overwrites 
                _students.ReplaceOne(st => st.studentID == student.studentID, student);
                message = null;
                return true; // success
            }
            else
                message = "student not found to update";

            return false; // return failure

        }

        /*  Validates the property values/ranges in student class. 
            Returns true and message = null if the validation passes. 
            Returns false and message = error message if the validation fails. 
        */
        private bool ValidateStudent(Student student, out string message)
        {
            if (!ValidateStudentID(student.studentID))
            {
                message = "studentID must be a 6 digit number";
                return false;
            }
            if (!ValidateDepartment(student.department))
            {
                message = "Department should be from : " + string.Join(" ",departments);
                return false;
            }
            if (!ValidateGrade(student.grade))
            {
                message = "grade should be from : " + string.Join(" ", grades);
                return false;
            }

            message = null;
            return true;
        }

        /*  Validates the studentID property in student class. 
            Returns true if the validation passes. 
            Returns false  if the validation fails. 
        */
        private bool ValidateStudentID(string sId)
        {
            if (sId.Length != studentIDLength)
                return false;
            for (int i = 0; i < sId.Length; i++)
                if (!char.IsDigit(sId[i]))
                    return false;

            return true;

        }

        /*  Validates the grade property in student class. 
            Returns true if the validation passes. 
            Returns false  if the validation fails. 
        */
        private bool ValidateGrade(string grade)
        {
            if (grade.Length > maxGradeLength)
                return false;

            int i;
            for (i = 0; i < grades.Length; i++)
            {
                if (grade == grades[i])
                    break; // grade is a valid grade
            }
            if (i == grades.Length)  // grade is not in the grades list and validation fails
                return false;
            return true;
        }

        /*  Validates the department property in student class. 
            Returns true if the validation passes. 
            Returns false  if the validation fails. 
        */
        private bool ValidateDepartment(string dept)
        {
            int i;
            for (i = 0; i < departments.Length; i++)
            {
                if (dept == departments[i])
                    break; // dept is a valid department
            }
            if (i == departments.Length)  // department is not in the departments list  and validation fails
                return false;
            return true;
        }

    }
}