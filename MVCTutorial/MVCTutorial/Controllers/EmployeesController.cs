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
            ViewBag.Roles = new SelectList(Enumerable.Empty<SelectListItem>()); // empty initially
            return View();
        }
        [HttpGet]
        public JsonResult GetRolesByDepartment(int departmentId)
        {
            var roles = _context.Roles
                .Where(r => r.DepartmentId == departmentId)
                .Select(r => new { r.RoleId, r.Name })
                .ToList();

            return Json(roles);
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
                return RedirectToAction(nameof(NeonIndex));
            }
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "Name");
            ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.DepartmentId == employee.Role.DepartmentId), "RoleId", "Name", employee.RoleId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var employee = await _context.Employees
        .Include(e => e.Role)
        .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            // Get DepartmentId from the employee's role
            int departmentId = employee.Role.DepartmentId;

            ViewBag.Departments = new SelectList(
                                    _context.Departments,
                                    "DepartmentId",
                                    "Name",
                                     departmentId
                                                 );
            // Populate Roles for that department (pre-select role)
            ViewBag.Roles = new SelectList(
                _context.Roles.Where(r => r.DepartmentId == departmentId),
                "RoleId",
                "Name",
                employee.RoleId
            );
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? photo)
        {
            // Get DepartmentId based on the RoleId posted from the form
            int departmentId = _context.Roles
                .Where(r => r.RoleId == employee.RoleId)
                .Select(r => r.DepartmentId)
                .FirstOrDefault();

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
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.Salary = employee.Salary;
                existingEmployee.RoleId = employee.RoleId;


                // Handle photo removal or replacement
                if (removePhoto)
                {
                    existingEmployee.PhotoPath = "/images/employees/userPhoto.jpg";
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
                return RedirectToAction(nameof(NeonIndex));
            }
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "Name", departmentId);
            ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.DepartmentId == departmentId), "RoleId", "Name", employee.RoleId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
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
            var employee = await _context.Employees
    .Include(e => e.Role)
    .ThenInclude(r => r.Department)
    .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            // Get DepartmentId from the employee's role
            int departmentId = _context.Roles
                .Where(r => r.RoleId == employee.RoleId)
                .Select(r => r.DepartmentId)
                .FirstOrDefault();

            return View(employee);
        }

        // GET: Employees
        //This initialises neon theme
        public async Task<IActionResult> NeonIndex()
        {

            var employeesByDept = await _context.Departments
                .Include(d => d.Roles)
                    .ThenInclude(r => r.Employees)
                .ToListAsync();

            return View(employeesByDept);
        }

        // GET: Employees/CreateDepartment
        public async Task<IActionResult> Department()
        {
            var vm = new DepartmentPageVM
            {
                Departments = await _context.Departments.ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Department(DepartmentPageVM vm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vm.NewDepartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Department));
            }
            vm.Departments = await _context.Departments.ToListAsync();
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Roles)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return NotFound();

            // Prevent deletion of the "Unassigned" department
            if (department.Name == "Unassigned")
            {
                TempData["ErrorMessage"] = "The 'Unassigned' department cannot be deleted.";
                return RedirectToAction(nameof(Department));
            }

            // Find or create "Unassigned" department and role
            var unassignedDept = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name == "Unassigned");

            if (unassignedDept == null)
            {
                unassignedDept = new Department { Name = "Unassigned", Location = "N/A" };
                _context.Departments.Add(unassignedDept);
                await _context.SaveChangesAsync();
            }

            var unassignedRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Unassigned" && r.DepartmentId == unassignedDept.DepartmentId);

            if (unassignedRole == null)
            {
                unassignedRole = new Role { Name = "Unassigned", DepartmentId = unassignedDept.DepartmentId };
                _context.Roles.Add(unassignedRole);
                await _context.SaveChangesAsync();
            }

            // Reassign employees
            var employeesInDept = _context.Employees
                .Where(e => department.Roles.Select(r => r.RoleId).Contains(e.RoleId))
                .ToList();

            foreach (var emp in employeesInDept)
            {
                emp.RoleId = unassignedRole.RoleId;
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Department));
        }



    }
}

