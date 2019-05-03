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
    [PermissionChecker(9)]
    public class DeleteRoleModel : PageModel
    {
        private readonly IPermissionService permissionService;

        [BindProperty]
        public Role Role { get; set; }

        public DeleteRoleModel(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        public void OnGet(int id)
        {
            Role = permissionService.GetRoleById(id);
        }

        public IActionResult OnPost()
        {
            permissionService.DeleteRole(Role);
            return RedirectToPage("Index");
        }
    }
}