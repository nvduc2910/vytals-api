
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.Infrastructures
{
    public class ApiResponder
    {
        public Error error;
        public dynamic data;

        public ApiResponder(dynamic data, Error error)
        {
            this.error = error;
            this.data = data;
        }

        public static ObjectResult RespondFailureTo(int statusCode, dynamic errors, int errorCode)
        {
            ApiResponder apiResponder = new ApiResponder(null,
                new Error { errorMessage = errors, errorCode = errorCode });

            var objectResult = new ObjectResult(apiResponder) { StatusCode = statusCode };

            return objectResult;
        }

        public static ObjectResult RespondSuccessTo(int statusCode, dynamic data)
        {
            ApiResponder apiResponder = new ApiResponder(data, null);

            var objectResult = new ObjectResult(apiResponder) { StatusCode = statusCode };

            return objectResult;
        }
    }
}
