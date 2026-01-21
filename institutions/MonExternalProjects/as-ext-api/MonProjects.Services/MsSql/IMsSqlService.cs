namespace MonProjects.Services.MsSql
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IMsSqlService
    {
        Task<TModel> ExecuteFirstAsync<TModel>(string procedureName, object parameters = null);

        Task<IEnumerable<TModel>> ExecuteListAsync<TModel>(string procedureName, object parameters = null);
    }
}
