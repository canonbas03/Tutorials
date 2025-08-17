using LearnProject.Controllers;
using LearnProject.Data;
using LearnProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection.Metadata;

namespace LearnProject.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeContext _context;
        public EmployeeController(EmployeeContext context)
        {
            _context = context;
        }

        public IActionResult AddTest()
        {
            var employee = new Employee { FirstName = "Hakuna", LastName = "Matata" };
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return Content("Added 1 employee");
        }

        public IActionResult List(string searchString, string sortOrder, int page = 1)
        {
            var employees = _context.Employees.AsQueryable();
          
            if (!string.IsNullOrEmpty(searchString))
            {
               employees = employees.Where(e => e.FirstName.ToLower().Contains(searchString.ToLower()) || e.LastName.Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.FirstName);
                    break;
                case "salary":
                    employees = employees.OrderBy(e => e.Salary);
                    break;
                case "salary_desc":
                    employees = employees.OrderByDescending(e => e.Salary);
                    break;
                default:
                    employees = employees.OrderBy(e => e.FirstName);
                    break;
            }

            // Paging
            int pageSize = 2;
            var totalEmployees = employees.Count();
            var employeesPaged = employees.Skip((page - 1)*pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            return View(employeesPaged);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(employee);
        }

        public IActionResult Details(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null) return NotFound();
            return View(employee);
        }
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Update(employee);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null) return NotFound();
            return View(employee);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if(employee == null) return NotFound();
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}

// EmployeeController(controller) → Handles requests related to employees and decides what data to fetch or save.




















//private readonly EmployeeContext _context;
//public EmployeeController(EmployeeContext context)
//{
//    _context = context;
//}

//public IActionResult AddTest()
//{
//    var employee = new Employee { FirstName = "Hello", LastName = "Darling" };
//    _context.Employees.Add(employee);
//    _context.SaveChanges();
//    return Content("Added 1 employee");
//}

//public IActionResult List()
//{
//    var employees = _context.Employees.ToList();
//    return View(employees);
//}