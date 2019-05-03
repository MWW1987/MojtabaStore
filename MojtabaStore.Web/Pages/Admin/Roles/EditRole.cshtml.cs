using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Entities.User;
using System.Collections.Generic;

namespace MojtabaStore.Web.Pages.Admin.Roles
{
    [PermissionChecker(8)]
    public class EditRoleModel : PageModel
    {
        private readonly IPermissionService permissionService;

        [BindProperty]
        public Role Role { get; set; }

        public EditRoleModel(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        public void OnGet(int id)
        {
            Role = permissionService.GetRoleById(id);
            ViewData["Permissions"] = permissionService.GetAllPermission();
            ViewData["SelectedPermissions"] = permissionService.PermissionsRole(id);
        }

        public IActionResult OnPost(List<int> SelectedPermission)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            permissionService.UpdateRole(Role);
            permissionService.UpdatePermissionsRole(Role.RoleId, SelectedPermission);

            return RedirectToPage("Index");
        }
    }
}