using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;

namespace MojtabaStore.Web.Pages.Admin.Users
{
    [PermissionChecker(2)]
    public class ListDeleteUsersModel : PageModel
    {
        private readonly IUserService userService;

        public ListDeleteUsersModel(IUserService userService)
        {
            this.userService = userService;
        }

        public UserForAdminViewModel UserForAdminViewModel { get; set; }
        public void OnGet(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            UserForAdminViewModel = userService.GetDeleteUsers(pageId, filterEmail, filterUserName);
        }
    }
}