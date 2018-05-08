using System;
using Vytals.Infrastructures.Mappings;

namespace Vytals.Models.ReutrnModels
{
    public class UserProfileReturnModel : IMapFrom<ApplicationUser>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public int Point { get; set; }
        public string Name { get; set; }
    }
}
