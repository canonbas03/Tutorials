using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCTutorial.Data;
using MVCTutorial.Models;

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
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? photo)
        {
            // Log ModelState errors if any
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine($"ModelState error: {error.ErrorMessage}");
            }

            // Log the received DepartmentId
            Console.WriteLine($"Received DepartmentId: {employee.DepartmentId}");

            

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
                    employee.PhotoPath = "/images/employees/userPhoto.jpg"; // explicitly set null or just leave it
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = new SelectList(_context.Departments.ToList(), "DepartmentId", "Name", employee.DepartmentId);
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
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId);
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
                existingEmployee.DepartmentId = employee.DepartmentId;


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
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            // Remove photo file if not default image
            var photoFileName = Path.GetFileName(employee.PhotoPath);
            var photoPathTrimmed = employee.PhotoPath?.Trim();

            if (!string.IsNullOrEmpty(photoPathTrimmed) &&
                !photoPathTrimmed.EndsWith("userPhoto.jpg", StringComparison.OrdinalIgnoreCase))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoPathTrimmed.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int id)
        {
            //var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            var employee = await _context.Employees
       .Include(e => e.Department)  // Include the related Department data
       .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }



    }
}

