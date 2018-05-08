using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Vytals.CustomAttribute
{
    public class MatchForPassword : ValidationAttribute
    {
        private readonly string passwordProperty;
        private readonly string confirmedPasswordProperty;

        public MatchForPassword(string passwordProperty, string confirmedPasswordProperty)
        {
            this.passwordProperty = passwordProperty;
            this.confirmedPasswordProperty = confirmedPasswordProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string password = GetValue<string>(validationContext, passwordProperty);
            string confirmedPassword = GetValue<string>(validationContext, confirmedPasswordProperty);

            if (confirmedPassword != password)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        private T GetValue<T>(ValidationContext validationContext, string propertyName)
        {
            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(propertyName);
            return (T)propertyInfo.GetValue(validationContext.ObjectInstance);
        }
    }
}
