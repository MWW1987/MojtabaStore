using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Entities.User;

namespace MojtabaStore.Web.Pages.Admin.Roles
{
    [PermissionChecker(7)]
    public class CreateRoleModel : PageModel
    {
        private readonly IPermissionService permissionService;

        public CreateRoleModel(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        [BindProperty]
        public Role Role { get; set; }

        public void OnGet()
        {
            ViewData["Permissions"] = permissionService.GetAllPermission();
        }

        public IActionResult OnPost(List<int> SelectedPermission)
        {
            if (!ModelState.IsValid)
                return Page();

            Role.IsDelete = false;
            int roleId = permissionService.AddRole(Role);
            permissionService.AddPermissionToRole(roleId, SelectedPermission);
            return RedirectToPage("Index");
        }
    }
}