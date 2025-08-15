using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCTutorial.Models
{
    public class AssignRolesVM
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        // Selected role IDs for checkboxes
        public List<int> SelectedRoleIds { get; set; } = new List<int>();

        // All roles for the checkboxes
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        // New role to create
        public string NewRoleName { get; set; }
    }
}
