using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Entities.User;
using System.Collections.Generic;

namespace MojtabaStore.Web.Pages.Admin.Roles
{
    [PermissionChecker(6)]
    public class IndexModel : PageModel
    {
        private readonly IPermissionService permissionService;

        public List<Role> RolesList { get; set; }

        public IndexModel(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        public void OnGet()
        {
            RolesList = permissionService.GetRoles();
        }
    }
}