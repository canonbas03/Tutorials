using Microsoft.AspNetCore.Mvc;

namespace MVCTutorial.Models
{
    public class DepartmentPageVM
    {
        public Department NewDepartment { get; set; } = new Department();
        public IEnumerable<Department> Departments { get; set; } = new List<Department>();
    }
}
