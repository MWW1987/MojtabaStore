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
    [PermissionChecker(5)]
    public class DeleteUserModel : PageModel
    {
        private readonly IUserService userService;
        public InformationUserViewModel InformationUserViewModel { get; set; }
        public DeleteUserModel(IUserService userService)
        {
            this.userService = userService;
        }

        

        public void OnGet(int id)
        {
            ViewData["UserId"] = id;
            InformationUserViewModel = userService.GetUserInformation(id);
        }

        public IActionResult OnPost(int UserId)
        {
            userService.DeleteUser(UserId);
            return RedirectToPage("Index");
        }
    }
}