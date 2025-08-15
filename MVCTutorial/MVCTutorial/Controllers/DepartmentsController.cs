using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCTutorial.Data;
using MVCTutorial.Models;

public class DepartmentsController : Controller
{
    private readonly EmployeeContext _context;
    public DepartmentsController(EmployeeContext context)
    {
        _context = context;
    }

    // GET: Departments/AssignRoles
    public async Task<IActionResult> AssignRoles()
    {
        // Pass list of departments for dropdown
        ViewBag.Departments = await _context.Departments
    .Where(d => d.Name != "Unassigned")
    .Select(d => new SelectListItem
    {
        Value = d.DepartmentId.ToString(),
        Text = d.Name
    }).ToListAsync();

        return View(new AssignRolesVM()); // empty VM initially
    }
    // GET: Departments/GetRolesByDepartment?departmentId=1
    public async Task<IActionResult> GetRolesByDepartment(int departmentId)
    {
        var roles = await _context.Roles
            .Where(r => r.DepartmentId == departmentId)
            .Select(r => new { roleId = r.RoleId, name = r.Name })
            .ToListAsync();

        return Json(roles);
    }

    // POST: Departments/AssignRoles
    [HttpPost]
    public async Task<IActionResult> AssignRoles(AssignRolesVM vm)
    {
        // 1️⃣ Load the department with its roles and employees
        var department = await _context.Departments
            .Include(d => d.Roles)
            .ThenInclude(r => r.Employees)
            .FirstOrDefaultAsync(d => d.DepartmentId == vm.DepartmentId);

        if (department == null) return NotFound();

        // 2️⃣ Ensure "Unassigned" role exists
        var unassignedRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.DepartmentId == department.DepartmentId && r.Name == "Unassigned");

        if (unassignedRole == null)
        {
            unassignedRole = new Role
            {
                Name = "Unassigned",
                DepartmentId = department.DepartmentId
            };
            _context.Roles.Add(unassignedRole);
            await _context.SaveChangesAsync();
        }

        // 3️⃣ Reassign employees from removed roles to "Unassigned"
        var removedRoles = department.Roles
            .Where(r => !vm.SelectedRoleIds.Contains(r.RoleId))
            .ToList();


        foreach (var role in removedRoles)
        {
            if (role.Employees == null) continue; // <-- guard null
            foreach (var emp in role.Employees)
            {
                emp.RoleId = unassignedRole.RoleId;
            }
        }

        // 4️⃣ Clear old roles and assign selected roles
        department.Roles.Clear();
        var selectedRoles = await _context.Roles
            .Where(r => vm.SelectedRoleIds.Contains(r.RoleId))
            .ToListAsync();

        // Always include Unassigned role
        if (!selectedRoles.Any(r => r.RoleId == unassignedRole.RoleId))
        {
            selectedRoles.Add(unassignedRole);
        }

        foreach (var role in selectedRoles)
            department.Roles.Add(role);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Roles updated successfully!";
        return RedirectToAction("AssignRoles"); }

        // GET: Departments/CreateRole?departmentId=1
public async Task<IActionResult> CreateRole(int departmentId)
    {
        var department = await _context.Departments.FindAsync(departmentId);
        if (department == null) return NotFound();

        ViewBag.DepartmentName = department.Name;
        ViewBag.DepartmentId = departmentId;
        return View();
    }

    // POST: Departments/CreateRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(int departmentId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            TempData["Error"] = "Role name cannot be empty.";
            return RedirectToAction("AssignRoles");
        }

        var department = await _context.Departments.FindAsync(departmentId);
        if (department == null) return NotFound();

        // Check if role already exists
        var existingRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.DepartmentId == departmentId && r.Name == roleName);
        if (existingRole != null)
        {
            TempData["Error"] = $"Role '{roleName}' already exists in {department.Name}.";
            return RedirectToAction("AssignRoles");
        }

        var newRole = new Role
        {
            Name = roleName,
            DepartmentId = departmentId
        };
        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Role '{roleName}' added to {department.Name}.";
        return RedirectToAction("AssignRoles");
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
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        // Step 1: Get or create the Unassigned department
        var unassignedDept = await _context.Departments
            .FirstOrDefaultAsync(d => d.Name == "Unassigned");

        if (unassignedDept == null)
        {
            unassignedDept = new Department { Name = "Unassigned", Location = "N/A" };
            _context.Departments.Add(unassignedDept);
            await _context.SaveChangesAsync();
        }

        // Step 2: Get or create the Unassigned role
        var unassignedRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.DepartmentId == unassignedDept.DepartmentId && r.Name == "Unassigned");

        if (unassignedRole == null)
        {
            unassignedRole = new Role
            {
                Name = "Unassigned",
                DepartmentId = unassignedDept.DepartmentId
            };
            _context.Roles.Add(unassignedRole);
            await _context.SaveChangesAsync();
        }

        // Step 3: Prevent deleting the Unassigned department
        if (id == unassignedDept.DepartmentId)
        {
            return BadRequest("Cannot delete the Unassigned department.");
        }

        // Step 4: Move employees to the Unassigned role
        var employeesToMove = await _context.Employees
            .Where(e => e.Role.DepartmentId == id)
            .ToListAsync();

        foreach (var emp in employeesToMove)
        {
            emp.RoleId = unassignedRole.RoleId;
        }

        // Step 5: Delete the department
        var department = await _context.Departments
            .Include(d => d.Roles)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);

        if (department != null)
        {
            _context.Roles.RemoveRange(department.Roles);
            _context.Departments.Remove(department);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Department));
    }

}

