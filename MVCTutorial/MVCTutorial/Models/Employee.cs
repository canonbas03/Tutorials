namespace MVCTutorial.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }

        // Path to employee image
        public string? PhotoPath { get; set; }
    }
}
