using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACHE.WebClientes.Models;

namespace ACHE.WebClientes
{
    public class LoggedOrAuthorizedAttribute : AuthorizeAttribute
    {
        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            CheckIfUserIsAuthenticated(filterContext);
        }

        private void CheckIfUserIsAuthenticated(AuthorizationContext filterContext)
        {
            // If Result is null, we’re OK: the user is authenticated and authorized. 
            if (filterContext.Result == null)
                return;

            // If here, you’re getting an HTTP 401 status code. In particular,
            // filterContext.Result is of HttpUnauthorizedResult type. Check Ajax here. 
            if (CurrentUser == null)//if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var helper = new UrlHelper(filterContext.RequestContext);
                var url = helper.Action("Login", "Account");
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}