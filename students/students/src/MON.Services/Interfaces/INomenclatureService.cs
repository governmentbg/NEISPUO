namespace MON.Services.Interfaces
{
    using MON.Models.Nomenclatures;
    using System.Threading.Tasks;

    public interface INomenclatureService
    {
        Task<IDNameModel> GetCountryByCode(string code);
        Task<IDNameModel> GetTownByName(string name);
    }
}
