    using AutoMapper;
    using NewDoor.Platform.DTO.Features.Rules.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.Rules.Mapper
    {
        public class RuleMapper : Profile
        {
            public RuleMapper()
            {
                CreateMap<AddRuleRequest, Rule>();
                CreateMap<Rule, RuleResponse>();
            }
        }
    }