    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IRuleConfigurationRepository : IBaseRepository<RuleConfiguration>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<RuleConfiguration> ruleConfigurations);
        Task<List<RuleConfiguration>> GetAllFilteredAsync(RuleConfigurationFilterRequest filter);
    }