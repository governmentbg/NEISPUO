namespace MON.Services.Implementations
{
    using Microsoft.Extensions.Caching.Memory;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.LeadTeacher;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using MON.Services;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using Z.Expressions.CodeCompiler.CSharp.Helper;
    using System.Linq.Dynamic.Core;

    public class LeadTeacherService : BaseService<LeadTeacherService>, ILeadTeacherService
    {
        private readonly IInstitutionService _institutionService;

        public LeadTeacherService(DbServiceDependencies<LeadTeacherService> dependencies, IInstitutionService institutionService)
            : base(dependencies)
        {
            _institutionService = institutionService;
        }

        public async Task Delete(int classId)
        {
            var leadTeacher = await _context.LeadTeachers.FirstOrDefaultAsync(i => i.ClassId == classId);
            if (leadTeacher != null)
            {
                _context.LeadTeachers.Remove(leadTeacher);
                await SaveAsync();
            }
        }
        public async Task<LeadTeacherViewModel> GetByClassId(int classId)
        {
            var leadTeacher = await (
                from lt in _context.LeadTeachers
                where lt.ClassId == classId
                select new LeadTeacherViewModel()
                {
                    Id = lt.Id,
                    LeadTeacherName = lt.StaffPosition.Person.FirstName + " " + lt.StaffPosition.Person.LastName,
                    ClassGroupName = lt.Class.ClassName,
                    StaffPositionId = lt.StaffPositionId,
                    CurrentlyValid = lt.StaffPosition.CurrentlyValid,
                    ClassId = lt.ClassId
                }).FirstOrDefaultAsync();

            if (leadTeacher == null)
            {
                var classGroup = await _context.ClassGroups.FirstOrDefaultAsync(i=> i.ClassId == classId);
                leadTeacher = new LeadTeacherViewModel()
                {
                    ClassId = classId,
                    ClassGroupName = classGroup?.ClassName
                };
            }

            return leadTeacher;
        }

        public Task<LeadTeacherViewModel> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedList<LeadTeacherViewModel>> List(PagedListInput input)
        {
            short currentYear = (await _institutionService.GetCurrentYear(_userInfo.InstitutionID));
            var query = (from l in _context.VLeadTeachers
                         where l.InstitutionId == _userInfo.InstitutionID
                         && l.SchoolYear == currentYear
                         select new LeadTeacherViewModel()
                         {
                             Id = l.LeadTeacherId,
                             LeadTeacherName = l.FullName,
                             ClassGroupName = l.ClassName,
                             StaffPositionId = l.StaffPositionId,
                             ClassId = l.ClassId
                         })
                        .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "ClassId desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            List<LeadTeacherViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task Update(LeadTeacherModel model)
        {
            var leadTeacher = await _context.LeadTeachers.FirstOrDefaultAsync(i => i.ClassId == model.ClassId);
            if (leadTeacher == null)
            {
                leadTeacher = new LeadTeacher() {ClassId = model.ClassId };
                _context.LeadTeachers.Add(leadTeacher);
            }
            leadTeacher.StaffPositionId = model.StaffPositionId.Value;
            await SaveAsync();
        }
    }
}
