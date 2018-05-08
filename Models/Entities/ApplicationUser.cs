using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Vytals.Enums;
using Microsoft.AspNetCore.Identity;
using Vytals.Models.Entities;

namespace Vytals.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int Level { get; set; }
        public int Point { get; set; }
        public int CurrentPointInGame { get; set; }
        public string DeviceToken { get; set; }
        public int EnemyId { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public UserState State { get; set; }
        public string Name { get; set; }
        public string MathType { get; set; }
        public string JwtToken { get; set; }

        public virtual ICollection<UserMedal> UserMedal { get; set; }
    }
}
