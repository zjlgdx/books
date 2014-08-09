using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CustomAuthorize.Models;

namespace CustomAuthorize.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            bool isAuthenticated = Authenticate(filterContext.HttpContext);

            if (!isAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new
                     RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                return;
            }

            // AuthorizeCore is in the base class and does the work of checking if we have
            // specified users or roles when we use our attribute
            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                     RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }

        private bool Authenticate(HttpContextBase context)
        {
            // Gets the <see cref="T:System.Security.Principal.WindowsIdentity"/> type for the current user.
            var windowsLogonUserName = string.Empty;
            if (System.Web.HttpContext.Current.Request.LogonUserIdentity != null)
            {
                windowsLogonUserName = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name;
            }

            var user = GetUserFromDatabaseByUserName(userName: windowsLogonUserName);

            if (user != null)
            {
                var principal = new GenericPrincipal(new GenericIdentity(user.Username), user.Roles.Select(r=>r.RoleName).ToArray());
                HttpContext.Current.User = principal;
                return true;
            }
            
            return false;
        }

        private User GetUserFromDatabaseByUserName(string userName)
        {
            IUserRepository repository= new HardCodedUserRepository();
            User user = repository.Authenticate(userName);
            return user;
        }
    }

    
}