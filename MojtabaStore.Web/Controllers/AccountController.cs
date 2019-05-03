using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MojtabaStore.Core.Convertors;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Generator;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Senders;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MojtabaStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IViewRenderService viewRender;

        public AccountController(IUserService userService, IViewRenderService viewRender)
        {
            this.userService = userService;
            this.viewRender = viewRender;
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
                return View(register);

            if (userService.IsExistUserName(register.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری قبلا ثبت شده است");
                return View(register);
            }

            if (userService.IsExistEmail(FixedText.FixEmail(register.Email)))
            {
                ModelState.AddModelError("UserName", "ایمیل  قبلا ثبت شده است");
                return View(register);
            }

            User user = new User()
            {
                ActiveCode = NameGenerator.GenerateUniqueCode(),
                Email = FixedText.FixEmail(register.Email),
                IsActive = false,
                Password = PasswordHelper.EncodePasswordMd5(register.Password),
                RegisterDate = DateTime.Now,
                UserAvatar = "Defult.jpg",
                UserName = register.UserName
            };

            userService.AddUser(user);
            string body = viewRender.RenderToStringAsync("_ActiveEmail", user);
            SendEmail.Send(user.Email, "فعالسازی", body);

            return View("SuccessRegister",user);
        }


        [Route("Login")]
        public IActionResult Login(bool editProfile = false)
        {
            ViewBag.EditProfile = editProfile;
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = userService.LoginUser(login);
            if (user != null)
            {
                if (user.IsActive)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var property = new AuthenticationProperties
                    {
                        IsPersistent = login.RememberMe
                    };

                    HttpContext.SignInAsync(principal, property);

                    ViewBag.IsSuccess = true;
                    return Redirect("/");
                }

                ModelState.AddModelError("Email", "حساب کاربری شما فعال نمیباشد");
                return View(login);
            }

            ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
            return View(login);
        }

        public IActionResult ActiveAccount(string id)
        {
            ViewBag.IsActive = userService.ActiveAccount(id);
            return View();
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }

        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (!ModelState.IsValid)
                return View(forgot);

            string fixedEmail = FixedText.FixEmail(forgot.Email);
            User user = userService.GetUserByEmail(fixedEmail);

            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری با این ایمیل یافت نشد");
                return View(forgot);
            }

            string bodyEmail = viewRender.RenderToStringAsync("_ForgotPassword", user);
            SendEmail.Send(user.Email, "بازیابی رمز عبور", bodyEmail);
            ViewBag.IsSuccess = true;

            return View();
        }

        public IActionResult ResetPassword(string id)
        {
            return View(new ResetPasswordViewModel()
            {
                ActiveCode = id
            });
        }


        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel reset)
        {
            if (!ModelState.IsValid)
                return View(reset);

            User user = userService.GetUserByActiveCode(reset.ActiveCode);
            if (user == null)
                return NotFound();

            string hashPassword = PasswordHelper.EncodePasswordMd5(reset.Password);
            user.Password = hashPassword;
            userService.UpdateUser(user);

            return Redirect("/Login");
        }
    }
}