using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using Golf.Domain.GolferData;

namespace Golf.HttpApi.Host.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public readonly string[] _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var golfer = (Golfer)context.HttpContext.Items["Golfer"];

            if (golfer == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                var roles = (List<string>)context.HttpContext.Items["Roles"];

                if (_roles.Length > 0)
                {
                    var isAuthorize = false;
                    foreach (var role in roles)
                    {
                        if (_roles.Contains(role))
                        {
                            isAuthorize = true;
                            break;
                        }
                    }
                    if (!isAuthorize)
                    {
                        context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                    }
                }
            }
        }
    }
}
