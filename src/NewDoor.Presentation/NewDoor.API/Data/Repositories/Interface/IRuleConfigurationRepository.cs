    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Repositories.Interface;

    public interface IRuleConfigurationRepository : IBaseRepository<RuleConfiguration>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<RuleConfiguration> ruleConfigurations);
    }