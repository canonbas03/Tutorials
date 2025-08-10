using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace MVCTutorial.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal Salary { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public string? PhotoPath { get; set; }

    }
}
