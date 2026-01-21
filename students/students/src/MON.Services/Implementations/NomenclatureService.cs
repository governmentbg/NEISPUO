namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.Models.Nomenclatures;
    using MON.Services.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;

    public class NomenclatureService : BaseService<NomenclatureService>, INomenclatureService
    {
        public NomenclatureService(DbServiceDependencies<NomenclatureService> dependencies)
            : base(dependencies)
        {
        }

        public async Task<IDNameModel> GetCountryByCode(string code)
        {
            var country =
                await (from c in _context.Countries
                       where c.Code == code
                       select new IDNameModel()
                       {
                           Id = c.CountryId,
                           Code = c.Code,
                           Name = c.Name
                       }).FirstOrDefaultAsync();
            return country;
        }

        public async Task<IDNameModel> GetTownByName(string name)
        {
            var towns =
                await (from t in _context.Towns
                       where t.Name == name
                       select new IDNameModel()
                       {
                           Id = t.TownId,
                           Name = $"гр./с.{t.Name} общ.{t.Municipality.Name} обл.{t.Municipality.Region.Name}"
                       }).ToListAsync();

            if (towns.Count == 1)
            {
                return towns.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
