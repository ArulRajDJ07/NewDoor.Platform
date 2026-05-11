    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;

    namespace NewDoor.API.Data.Repositories;

    public class RuleRepository(DoWhattaProductDBContext context)
        : BaseRepository<Rule>(context), IRuleRepository
    {
    }