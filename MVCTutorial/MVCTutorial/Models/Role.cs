namespace MVCTutorial.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
