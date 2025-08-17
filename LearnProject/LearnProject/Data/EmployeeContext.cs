using LearnProject.Data;
using LearnProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LearnProject.Data
{
    public class EmployeeContext : DbContext
    {
      public EmployeeContext(DbContextOptions options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
    }
}

// EmployeeContext (DbContext) → Represents the database and gives you access to the Employees table.























//public EmployeeContext(DbContextOptions options) : base(options) { }
//public DbSet<Employee> Employees { get; set; }