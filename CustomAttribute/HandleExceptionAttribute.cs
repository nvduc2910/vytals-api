using Vytals.Exceptions;
using Vytals.Helpers;
using Vytals.Infrastructures;
using Vytals.Resources;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Vytals.CustomAttribute
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is FailedModelValidationException)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.UnprocessableEntity,
                                                               new[] { context.Exception.Message }, ErrorDefine.INVALID_MODEL);
            if (context.Exception is FailedRegistrationException)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.NotImplemented,
                                                               new[] { context.Exception.Message }, ErrorDefine.REGISTER_FAIL);
            else if (context.Exception is UserNotExistsException)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.NotFound,
                                                               new[] { context.Exception.Message }, ErrorDefine.USER_NOT_FOUND);
            else if (context.Exception is IncorrectPasswordException)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.NotFound,
                                                               new[] { context.Exception.Message }, ErrorDefine.INCORECT_PASSWORD);

            else if (context.Exception is InvalidPinCodeException)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.BadRequest,
                                                               new[] { context.Exception.Message }, ErrorDefine.INVALID_PIN_CODE);

            else if (context.Exception is PinCodeExpiredException)
                context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.BadRequest,
                                                               new[] { context.Exception.Message }, ErrorDefine.PIN_CODE_EXPIRED);

            else
            context.Result = ApiResponder.RespondFailureTo(HttpStatusCode.InternalServerError,
                                                           new[] {context.Exception.Message}, ErrorDefine.GENERAL_CODE);
        }
    }
}
