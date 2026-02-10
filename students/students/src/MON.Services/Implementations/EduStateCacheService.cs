using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MON.DataAccess;
using MON.Models.EduState;
using MON.Models.Enums;
using MON.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class EduStateCacheService
    {
        private readonly MONContext _context;
        private readonly ICacheService _cache;

        public EduStateCacheService(MONContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<EduStateModel>> GetEduStatesForStudent(int studentId)
        {
            string key = $"{CacheKeys.EduStatesForStudent}_{studentId}";
            List<EduStateModel> list = await _cache.GetAsync<List<EduStateModel>>(key);

            if (list == null)
            {
                list = await _context.EducationalStates
                    .Where(x => x.PersonId == studentId && x.PositionId != (int)PositionType.Staff)
                    .Select(x => new EduStateModel
                    {
                        PersonId = x.PersonId,
                        PositionId = x.PositionId,
                        InstitutionId = x.InstitutionId
                    })
                    .ToListAsync();

                await _cache.SetAsync(key, list);
            }

            return list;
        }

        public async Task<List<EduStateModel>> GetEduStatesForPerson(string personalId)
        {
            string key = $"{CacheKeys.EduStatesForPersonal}_{personalId}";
            List<EduStateModel> list = await _cache.GetAsync<List<EduStateModel>>(key);

            if (list == null)
            {
                list = await _context.EducationalStates
                    .Where(x => x.Person.PersonalId == personalId && x.PositionId != (int)PositionType.Staff)
                    .Select(x => new EduStateModel
                    {
                        PersonId = x.PersonId,
                        PositionId = x.PositionId,
                        InstitutionId = x.InstitutionId,
                        PersonalId = x.Person.PersonalId
                    })
                    .ToListAsync();

                await _cache.SetAsync(key, list);
            }

            return list;
        }

        public async Task ClearEduStatesForStudent(int studentId)
        {
            string key = $"{CacheKeys.EduStatesForStudent}_{studentId}";
            //return _cache.SetAsync(key, default(List<EduStateModel>));
            await _cache.RemoveAsync(key);
        }

        public async Task ClearEduStatesForPerson(int personalId)
        {
            string key = $"{CacheKeys.EduStatesForPersonal}_{personalId}";
            //return _cache.SetAsync(key, default(List<EduStateModel>));
            await _cache.RemoveAsync(key);
        }

        public Task Clear()
        {
            return _cache.ClearCache();
        }
    }
}
