using Microsoft.AspNetCore.Mvc;
using MVCTutorial.Data;
using MVCTutorial.Models;
using Microsoft.EntityFrameworkCore;

namespace MVCTutorial.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeContext _context;

        public EmployeesController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }
    }
}

