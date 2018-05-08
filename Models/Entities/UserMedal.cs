using System;
using Vytals.Enums;

namespace Vytals.Models.Entities
{
    public class UserMedal
    {
        public int Id { get; set; }
        public Medal Madel { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
