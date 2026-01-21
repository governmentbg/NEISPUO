namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using System;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;

    public class BasicDocumentPartsService : BaseService<BasicDocumentPartsService>, IBasicDocumentPartsService
    {
        public BasicDocumentPartsService(DbServiceDependencies<BasicDocumentPartsService> dependencies)
            : base(dependencies)
        {
        }

        public Task<DiplomaTypeSchemaModel> GetSchemaById(int id)
        {
            return _context.BasicDocuments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new DiplomaTypeSchemaModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    BasicDocumentParts = x.BasicDocumentParts.Select(p => new BasicDocumentPartModel
                    {
                        Id = p.Id,
                        Position = p.Position,
                        Name = p.Name,
                        Description = p.Description,
                        Code = p.Code,
                        BasicClass = p.BasicClass,
                        BasicSubjectTypeId = p.BasicSubjectTypeId,
                        IsHorariumHidden = p.IsHorariumHidden,
                        Uid = Guid.NewGuid().ToString(),
                        BasicDocumentSubjects = p.BasicDocumentSubjects.Select(s => new BasicDocumentSubjectModel
                        {
                            Id = s.Id,
                            Position = s.Position,
                            SubjectCanChange = s.SubjectCanChange,
                            SubjectDropDown = new DropdownViewModel
                            {
                                Value = s.SubjectId.Value,
                                Name = s.Subject.SubjectName,
                                Text = s.Subject.SubjectName
                            },
                            Uid = Guid.NewGuid().ToString(),
                        }).ToList()
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }
    }
}
