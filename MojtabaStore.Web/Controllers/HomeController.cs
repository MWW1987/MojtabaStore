using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MojtabaStore.Core.Services.Interfaces;
using System.Collections.Generic;

namespace MojtabaStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly ICourseService courseService;

        public HomeController(IUserService userService, ICourseService courseService)
        {
            this.userService = userService;
            this.courseService = courseService;
        }

        public IActionResult Index() => View();

        [Route("OnlinePayment/{id}")]
        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
               HttpContext.Request.Query["Status"].ToString().ToLower() == "ok"
               && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];

                var wallet = userService.GetWalletByWalletId(id);

                var payment = new ZarinpalSandbox.Payment(wallet.Amount);
                var res = payment.Verification(authority).Result;
                if (res.Status == 100)
                {
                    ViewBag.code = res.RefId;
                    ViewBag.IsSuccess = true;
                    wallet.IsPay = true;
                    userService.UpdateWallet(wallet);
                }

            }

            return View();
        }

        public IActionResult GetSubGroups(int id)
        {
            List<SelectListItem> list = new List<SelectListItem>()
            {
                new SelectListItem() { Text ="انتخاب کنید", Value = ""}
            };
            list.AddRange(courseService.GetSubGroupForManageCourse(id));
            return Json(new SelectList(list, "Value", "Text"));
        }
    }
}