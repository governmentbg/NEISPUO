using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models.StudentModels.History;
using MON.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class HistoryService : BaseService<HistoryService>, IHistoryService
    {
        public HistoryService(DbServiceDependencies<HistoryService> dependencies) 
            : base(dependencies)
        {
        }

        public async Task<IEnumerable<StudentResourceSupportHistoryModel>> GetStudentResourceSupportHistoryRecords(int personId)
        {
            var result = await _context.ResourceSupports.FromSqlRaw("SELECT * FROM [student].[ResourceSupport] FOR SYSTEM_TIME All where PersonID = @personId", new SqlParameter("@personId", personId))
                 .Select(r => new StudentResourceSupportHistoryModel
                 {
                     ResourceSupportType = r.ResourceSupportType.Name
                 }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<StudentResourceSopDetails>> GetStudentResourceSopHistoryRecords(int personId)
        {
            IQueryable<StudentResourceSopDetails> query = _context.Set<StudentResourceSopDetails>()
                .FromSqlInterpolated($"SELECT * FROM [student].[v_sopDetails] FOR SYSTEM_TIME All where PersonID = {personId}")
                .AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<StudentPersonalDataHistoryModel>> GetStudentPersonalDataHistoryRecords(int personId)
        {
            IEnumerable<StudentPersonalDataHistoryModel> result = await _context.People.FromSqlRaw("SELECT * FROM [core].[Person] FOR SYSTEM_TIME All where PersonID = @personId", new SqlParameter("@personId", personId))
                 .Select(p => new StudentPersonalDataHistoryModel
                 {
                     FirstName = p.FirstName,
                     MiddleName = p.MiddleName,
                     LastName = p.LastName,
                     BirthDate = p.BirthDate,
                     PermanentAddress = p.PermanentAddress,
                     BirthPlace = p.BirthPlaceTown.Name,
                     CurrentAddress = p.CurrentAddress,
                     Gender = p.GenderNavigation.Name,
                     Nationality = p.Nationality.Name,
                     PhoneNumber = p.Student.MobilePhone
                 }).ToListAsync();

            return result;
        }
    }
}
