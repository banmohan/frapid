using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Frapid.Account.DAL;
using Frapid.Account.Models.Backend;
using Frapid.Account.ViewModels;
using Frapid.ApplicationState.Cache;
using Frapid.Dashboard;
using Frapid.Dashboard.Controllers;
using Frapid.DataAccess.Models;

namespace Frapid.Account.Controllers.Backend
{
    public class UserController : DashboardController
    {
        [Route("dashboard/account/user/list")]
        [MenuPolicy]
        [AccessPolicy("account", "users", AccessTypeEnum.Read)]
        public ActionResult Index()
        {
            return this.FrapidView(this.GetRazorView<AreaRegistration>("User/Index.cshtml", this.Tenant));
        }

        [Route("dashboard/account/my/change-password")]
        [MenuPolicy]
        public ActionResult ChangeMyPassword()
        {
            return this.FrapidView(this.GetRazorView<AreaRegistration>("User/ChangeMyPassword.cshtml", this.Tenant));
        }

        [Route("dashboard/account/my/change-password")]
        [HttpPost]
        public async Task<ActionResult> ChangeMyPasswordAsync(ChangePasswordInfo model)
        {
            var meta = await AppUsers.GetCurrentAsync(this.Tenant).ConfigureAwait(true);

            if (model.Password != model.ConfirmPassword)
            {
                return this.Failed(I18N.ConfirmPasswordDoesNotMatch, HttpStatusCode.BadRequest);
            }

            var user = await Users.GetAsync(this.Tenant, meta.UserId).ConfigureAwait(true);
            var isValid = user != null && PasswordManager.ValidateBcrypt(user.Email, model.CurrentPassword, user.Password);
            if (!isValid)
            {
                return this.Failed("Incorrect current password.", HttpStatusCode.BadRequest);
            }

            try
            {
                model.UserId = meta.UserId;
                model.Email = user.Email;
                await ChangePasswordModel.ChangePasswordAsync(this.Tenant, model).ConfigureAwait(true);
                return this.Ok("OK");
            }
            catch (Exception ex)
            {
                return this.Failed(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}