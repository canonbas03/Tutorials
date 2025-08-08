using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "The Department field is required.")]
        public int DepartmentId { get; set; }
        [BindNever]
        public Department? Department { get; set; } // Navigation property
    }
}
