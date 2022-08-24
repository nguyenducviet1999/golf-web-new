using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

using Golf.Domain.GolferData;
using Golf.Domain.Shared.Golfer;

namespace Golf.HttpApi.Host.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CourseAdminRouterProtector : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public CourseAdminRouterProtector(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var golfer = (Golfer)context.HttpContext.Items["Golfer"];
            var roles = (List<string>)context.HttpContext.Items["Roles"];

            if (roles.IndexOf(RoleNormalizedName.CourseAdmin) == -1)
                context.Result = new JsonResult(new { message = "Access Denied" }) { StatusCode = StatusCodes.Status405MethodNotAllowed };
        }
    }
}