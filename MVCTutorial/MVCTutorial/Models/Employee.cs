using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MVCTutorial.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }

        // Path to employee image
        [BindNever]
        public string? PhotoPath { get; set; }

        // New foreign key
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
