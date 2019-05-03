using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojtabaStore.Core.DTOs.User;
using MojtabaStore.Core.Services.Interfaces;

namespace MojtabaStore.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IUserService userService;

        public WalletController(IUserService userService)
        {
            this.userService = userService;
        }

        [Route("UserPanel/Wallet")]
        public IActionResult Index()
        {
            ViewBag.ListWallet = userService.GetWalletUser(User.Identity.Name);
            return View();
        }

        [Route("UserPanel/Wallet")]
        [HttpPost]
        public ActionResult Index(ChargeWalletViewModel charge)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListWallet = userService.GetWalletUser(User.Identity.Name);
                return View(charge);
            }

            int walletId = userService.ChargeWallet(User.Identity.Name, charge.Amount, "شارژ حساب");

            #region Online Payment

            var payment = new ZarinpalSandbox.Payment(charge.Amount);

            var res = payment.PaymentRequest("شارژ کیف پول", "https://localhost:44379/OnlinePayment/" + walletId, "m.mindhorizon@gmail.com", "09197070750");

            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }

            #endregion


            return null;
        }
    }
}