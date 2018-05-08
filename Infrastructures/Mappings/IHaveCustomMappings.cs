using System;
using AutoMapper;

namespace Vytals.Infrastructures.Mappings
{
    public interface IHaveCustomMappings
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
