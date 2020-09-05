using ProjectCRUD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectCRUD.Services
{
    public interface IStudentService
    {
        void Create(Student student);
        Student Get(string id);
        List<Student> GetAll();
        IEnumerable<object> GetDeptWiseCount();
        void Delete(string id);
        void Update(Student student);
    }
}
