using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper_Demo.Models.Entities;
using Vytals.Infrastructures.Mappings;
using Vytals.Resources;

namespace Vytals.Models.DTOModels
{
    public class RegisterDTO : IMapFrom<ApplicationUser>
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


        [Required(
            AllowEmptyStrings = false,
            ErrorMessageResourceType = typeof(ValidationModel),
            ErrorMessageResourceName = "NullEmptyName")]
        public string Name { get; set; }
    }
}
