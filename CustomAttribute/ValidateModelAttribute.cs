using Vytals.Helpers;
using Vytals.Infrastructures;
using Vytals.Resources;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Vytals.CustomAttribute
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {

        private readonly IStringLocalizer<ValidationModel> localizer;

        public ValidateModelAttribute()
        {
            
        }


        public ValidateModelAttribute(IStringLocalizer<ValidationModel> localizer)
        {
            this.localizer = localizer;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            List<string> validationErrors = ErrorBuilder.BuildInvalidModelStateError(context.ModelState);
            

            if (validationErrors != null && validationErrors.Count > 0)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.UnprocessableEntity, validationErrors,
                                                               ErrorDefine.INVALID_MODEL);
        }
    }
}
