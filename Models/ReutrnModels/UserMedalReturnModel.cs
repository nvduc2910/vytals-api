using System;
using AutoMapper;
using Vytals.Enums;
using Vytals.Infrastructures.Mappings;
using Vytals.Models.Entities;

namespace Vytals.Models.ReutrnModels
{
    public class UserMedalReturnModel : IHaveCustomMappings
    {
        public Medal Medal { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<UserMedal, UserMedalReturnModel>()
            .ForMember(m => m.Medal, opt => opt.MapFrom(u => u.Madel));
        }
    }
}
