namespace NeispuoExtension.Database.Services.NeispuoDatabase
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using DependencyInjection;

    public interface INeispuoDatabaseService : ITransientService
    {
        public Task<TModel> ExecuteFirstAsync<TModel>(string procedureName, object parameters = null);

        public Task<IEnumerable<TModel>> ExecuteListAsync<TModel>(string procedureName, object parameters = null);
    }
}
