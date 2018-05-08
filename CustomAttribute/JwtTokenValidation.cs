using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Vytals.Helpers;
using Vytals.Infrastructures;
using Vytals.Models;
using Vytals.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Vytals.CustomAttribute
{
    public class JwtTokenValidation : ActionFilterAttribute
    {
        private readonly IHttpContextAccessor httpCotext;
        private readonly IUnitOfWork unitOfWork;

        public JwtTokenValidation()
        {
           
        }

        public JwtTokenValidation(IHttpContextAccessor httpCotext, IUnitOfWork unitOfWork)
        {
            this.httpCotext = httpCotext;
            this.unitOfWork = unitOfWork;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues token;
            httpCotext.HttpContext.Request.Headers.TryGetValue("Authorization", out token);

            //if (token == string.Empty)
            //    context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.UnAuthorize, "UNAUTHORIZE",
            //                                                   ErrorDefine.UNAUTHORIZE);
            //else
            //{
            //    var userId = Convert.ToInt32(DecodingToken(token));
            //    if(userId != 0)
            //    {
            //        var usreInfo = unitOfWork.GetRepository<ApplicationUser>().Get(s => s.Id == userId).FirstOrDefault();

            //        if(usreInfo.JwtToken != token)
            //            context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.UnAuthorize, "UNAUTHORIZE",
            //                                                   ErrorDefine.UNAUTHORIZE);
            //    }
            //    else
            //        context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.UnAuthorize, "UNAUTHORIZE",
            //                                                   ErrorDefine.UNAUTHORIZE);
            //}


            var userId = Convert.ToInt32(DecodingToken(token));
            if(userId != 0)
            {
                var usreInfo = unitOfWork.GetRepository<ApplicationUser>().Get(s => s.Id == userId).FirstOrDefault();

                if(usreInfo.JwtToken != token)
                    context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.UnAuthorize, "UNAUTHORIZE",
                                                           ErrorDefine.UNAUTHORIZE);
            }
            base.OnActionExecuting(context);
        }

        public string DecodingToken(string token)
        {
            if (token != null && token != string.Empty)
            {
                token = token.Split(' ').LastOrDefault();
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var claimId = tokenS.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                return claimId;
            }
            return string.Empty;
        }
    }
}
