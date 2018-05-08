using System;
using System.ComponentModel.DataAnnotations;
using Vytals.Resources;

namespace Vytals.Models.DTOModels
{
    public class LoginDTO
    {
        [Required(
            AllowEmptyStrings = false,
            ErrorMessageResourceType = typeof(ValidationModel),
            ErrorMessageResourceName = "NullEmptyEmail")]
        [EmailAddress(
            ErrorMessageResourceType = typeof(ValidationModel),
            ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        [Required(
            AllowEmptyStrings = false,
            ErrorMessageResourceType = typeof(ValidationModel),
            ErrorMessageResourceName = "NullEmptyPassword")]
        public string Password { get; set; }

        public string DeviceToken { get; set; }
    }
}
