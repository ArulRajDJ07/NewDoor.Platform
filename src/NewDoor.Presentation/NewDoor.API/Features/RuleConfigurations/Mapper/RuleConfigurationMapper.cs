    using AutoMapper;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.RuleConfigurations.Mapper
    {
        public class RuleConfigurationMapper : Profile
        {
            public RuleConfigurationMapper()
            {
                CreateMap<AddRuleConfigurationRequest, RuleConfiguration>();
                CreateMap<RuleConfiguration, RuleConfigurationResponse>();
            }
        }
    }