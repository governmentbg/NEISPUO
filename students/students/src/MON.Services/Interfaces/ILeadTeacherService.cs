namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.LeadTeacher;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface ILeadTeacherService
    {
        Task<IPagedList<LeadTeacherViewModel>> List(PagedListInput input);

        Task Update(LeadTeacherModel model);

        Task<LeadTeacherViewModel> GetById(int id);
        Task<LeadTeacherViewModel> GetByClassId(int classId);

        Task Delete(int classId);
    }
}
