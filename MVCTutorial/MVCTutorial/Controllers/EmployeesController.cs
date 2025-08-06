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

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? photo)
        {
           // ModelState.Remove("Photo");
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    var fileName = Path.GetFileName(photo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/employees/", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    employee.PhotoPath = "/images/employees/" + fileName;
                }
                else
                {
                    employee.PhotoPath = "/images/employees/userPhoto.png"; // explicitly set null or just leave it
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? photo)
        {
            // Manually get removePhoto from form data as string
            var removePhotoStr = Request.Form["removePhoto"].FirstOrDefault();
            bool removePhoto = !string.IsNullOrEmpty(removePhotoStr) && removePhotoStr.ToLower() == "on";
            //bool removePhoto = !string.IsNullOrEmpty(removePhotoStr) && (removePhotoStr.ToLower() == "true" || removePhotoStr == "on");

            // Or better:
            // bool removePhoto = Request.Form["removePhoto"] == "on";

            if (ModelState.IsValid)
            {
                var existingEmployee = await _context.Employees.FindAsync(id);
                if (existingEmployee == null)
                    return NotFound();

                // Update fields
                existingEmployee.Name = employee.Name;
                existingEmployee.Salary = employee.Salary;

                // Handle photo removal or replacement
                if (removePhoto)
                {
                    existingEmployee.PhotoPath = "/images/employees/userPhoto.png";
                }
                else if (photo != null && photo.Length > 0)
                {
                    var fileName = Path.GetFileName(photo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/employees/", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    existingEmployee.PhotoPath = "/images/employees/" + fileName;
                }

                _context.Update(existingEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }




    }
}

