using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Vytals.Helpers
{
    public static class ErrorBuilder
    {
        public static List<string> BuildInvalidModelStateError(ModelStateDictionary modelState)
        {
            List<string> errors = new List<string>();

            if (!modelState.IsValid)
            {
                var allErrors = modelState.Values.SelectMany(v => v.Errors).ToList();
                errors = new List<string>();
                foreach (var error in allErrors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return errors;
        }
    }
}
