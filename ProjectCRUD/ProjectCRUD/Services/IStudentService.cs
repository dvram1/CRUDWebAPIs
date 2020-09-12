using ProjectCRUD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectCRUD.Services
{
    public interface IStudentService
    {
        bool Create(Student student, out string message);
        Student Get(string id, out string message);
        Student Get(string id);
        List<Student> GetAll();
        IEnumerable<object> GetDeptWiseCount();
        bool Delete(string id, out string message);
        bool Update(Student student, out string message);
    }
}
