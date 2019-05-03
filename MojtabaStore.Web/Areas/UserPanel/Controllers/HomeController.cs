using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Services.Interfaces;

namespace MojtabaStore.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService userService;

        public HomeController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View(userService.GetUserInformation(User.Identity.Name));
        }

        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(userService.GetDataForEditProfileUser(User.Identity.Name));
        }


        [Route("UserPanel/EditProfile")]
        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel profile)
        {
            if (!ModelState.IsValid)
                return View(profile);

            userService.EditProfile(User.Identity.Name, profile);


            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login?EditProfile=true");
        }

        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Route("UserPanel/ChangePassword")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel change)
        {
            string currentUserName = User.Identity.Name;

            if (!ModelState.IsValid)
                return View(change);

            if (!userService.CompareOldPassword(currentUserName, change.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور فعلی شما درست نیست");
                return View(change);
            }

            userService.ChangeUserPassword(currentUserName, change.Password);
            ViewBag.IsSuccess = true;
            return View();
        }
    }
}