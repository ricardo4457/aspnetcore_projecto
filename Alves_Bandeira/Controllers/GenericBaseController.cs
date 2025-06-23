using Alves_Bandeira.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Alves_Bandeira.Controllers {
    public class GenericBaseController : Controller {
        protected User? _user;

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            HelperUser helperUser = new HelperUser();

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user"))) {
                HttpContext.Session.SetString("user", helperUser.SerializeUser(helperUser.SetGuest()));
            }

            _user = helperUser.DeserializeUser(HttpContext.Session.GetString("user") ?? string.Empty);
        }
    }
}
