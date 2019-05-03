using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;

namespace MojtabaStore.Web.Pages.Admin.Users
{
    [PermissionChecker(3)]
    public class CreateUserModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;

        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            this.userService = userService;
            this.permissionService = permissionService;
        }


        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }

        public void OnGet()
        {
            ViewData["Roles"] = permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> SelectedRoles)
        {
            if (!ModelState.IsValid)
                return Page();

            int userId = userService.AddUserFromAdmin(CreateUserViewModel);

            //Add Roles
            permissionService.AddRolesToUser(SelectedRoles, userId);


            return Redirect("/Admin/Users");

        }
    }
}