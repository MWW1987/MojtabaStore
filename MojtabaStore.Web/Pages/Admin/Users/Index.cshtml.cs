using Microsoft.AspNetCore.Mvc.RazorPages;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;

namespace MojtabaStore.Web.Pages.Admin.Users
{
    [PermissionChecker(2)]
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;

        public IndexModel(IUserService userService)
        {
            this.userService = userService;
        }

        public UserForAdminViewModel UserForAdminViewModel { get; set; }
        public void OnGet(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            UserForAdminViewModel = userService.GetUsers(pageId, filterEmail, filterUserName);
        }

        
    }
}