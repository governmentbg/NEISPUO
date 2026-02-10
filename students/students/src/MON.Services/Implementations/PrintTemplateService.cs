namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Shared;
    using MON.Models.Institution.PrintTemplate;
    using MON.Services.Interfaces;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using MON.Models.Dropdown;
    using System.Threading;
    using MON.Services.Security.Permissions;

    public class PrintTemplateService : BaseService<PrintTemplateService>, IPrintTemplateService
    {
        private readonly IInstitutionService _institutionService;

        public PrintTemplateService(DbServiceDependencies<PrintTemplateService> dependencies, IInstitutionService institutionService)
            : base(dependencies)
        {
            _institutionService = institutionService;
        }

        private void Authorize(PrintTemplate template)
        {
            _ = template ?? throw new ApiException(Messages.EmptyEntityError);

            if (_userInfo.InstitutionID.HasValue)
            {
                if (template.InstitutionId != _userInfo.InstitutionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else if (_userInfo.RegionID.HasValue)
            {
                if (template.RuoRegId != _userInfo.RegionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        
        public async Task<List<PrintTemplateViewModel>> List(CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForPrintTemplatesShow))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<PrintTemplate> query = _context.PrintTemplates;

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            }
            else if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RuoRegId == _userInfo.RegionID.Value);
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<PrintTemplateViewModel> templates = await query
                .Select(x => new PrintTemplateViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    InstitutionId = x.InstitutionId,
                    RuoRegId = x.RuoRegId,
                    Left1Margin = x.Left1Margin,
                    Top1Margin = x.Top1Margin,
                    Left2Margin = x.Left2Margin,
                    Top2Margin = x.Top2Margin,
                    HasContents = x.Contents != null,
                    PrintFormId = x.BasicDocumentPrintFormId,
                    PrintFormEdition = x.BasicDocumentPrintForm.Edition,
                    BasicDocumentId = x.BasicDocumentId,
                    BasicDocumentName = x.BasicDocument.Name,
                    BasicDocument = new DropdownViewModel()
                    {
                        Name = x.BasicDocument.Name,
                        Value = x.BasicDocumentId,
                        Text = x.BasicDocument.Name
                    }
                })
                .ToListAsync(cancellationToken);

            return templates;
        }

        public async Task<PrintTemplateViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            PrintTemplateViewModel template = await (
                from p in _context.PrintTemplates
                where p.Id == id
                select new PrintTemplateViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    InstitutionId = p.InstitutionId,
                    RuoRegId = p.RuoRegId,
                    Left1Margin = p.Left1Margin,
                    Top1Margin = p.Top1Margin,
                    Left2Margin = p.Left2Margin,
                    Top2Margin = p.Top2Margin,
                    BasicDocumentId = p.BasicDocumentId,
                    BasicDocumentName = p.BasicDocument.Name,
                    PrintFormId = p.BasicDocumentPrintFormId,
                    PrintFormEdition = p.BasicDocumentPrintForm.Edition,
                    BasicDocument = new DropdownViewModel()
                    {
                        Name = p.BasicDocument.Name,
                        Value = p.BasicDocumentId,
                        Text = p.BasicDocument.Name
                    }
                }).FirstOrDefaultAsync(cancellationToken);

            if (_userInfo.InstitutionID.HasValue)
            {
                if (template.InstitutionId != _userInfo.InstitutionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else if (_userInfo.RegionID.HasValue)
            {
                if (template.RuoRegId != _userInfo.RegionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return template;
        }

        public async Task Create(PrintTemplateModel model)
        {
            _ = model ?? throw new ApiException(Messages.EmptyModelError);

            if (!_userInfo.InstitutionID.HasValue && !_userInfo.RegionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var dbPrintTemplate = new PrintTemplate()
            {
                BasicDocumentId = model.BasicDocumentId,
                Name = model.Name,
                Left1Margin = model.Left1Margin,
                Top1Margin = model.Top1Margin,
                Left2Margin = model.Left2Margin,
                Top2Margin = model.Top2Margin,
                Description = model.Description,
                InstitutionId = _userInfo.InstitutionID,
                RuoRegId = _userInfo.RegionID,
                SchoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID),
                BasicDocumentPrintFormId = model.PrintFormId,
            };

            var basicDocumentPrintForm = await _context.BasicDocumentPrintForms.FirstOrDefaultAsync(i => i.BasicDocumentId == model.BasicDocumentId && i.Id == model.PrintFormId);
            if (basicDocumentPrintForm?.ReportFormPath != null)
            {
                Assembly _thisAssembly = typeof(PrintTemplateService).Assembly;
                string _baseReportFilePath = Path.GetDirectoryName(_thisAssembly.Location);
                var reportContents = File.ReadAllBytes($@"{_baseReportFilePath}{Path.DirectorySeparatorChar}{basicDocumentPrintForm?.ReportFormPath}.trdp");
                dbPrintTemplate.Contents = reportContents;
            }

            _context.PrintTemplates.Add(dbPrintTemplate);
            await SaveAsync();
        }

        public async Task Update(PrintTemplateModel model)
        {
            _ = model ?? throw new ApiException(Messages.EmptyModelError);

            PrintTemplate template = await _context.PrintTemplates.SingleOrDefaultAsync(i => i.Id == model.Id);
            Authorize(template);

            template.BasicDocumentId = model.BasicDocumentId;
            template.Name = model.Name;
            template.Description = model.Description;
            template.Top1Margin = model.Top1Margin;
            template.Left1Margin = model.Left1Margin;
            template.Top2Margin = model.Top2Margin;
            template.Left2Margin = model.Left2Margin;
            template.BasicDocumentPrintFormId = model.PrintFormId;

            await SaveAsync();
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            PrintTemplate template = await _context.PrintTemplates.SingleOrDefaultAsync(i => i.Id == id);
            Authorize(template);

            _context.PrintTemplates.Remove(template);
            await SaveAsync(cancellationToken);
        }

        public async Task<List<PrintFormDropdownViewModel>> GetDropdownOptions(string searchStr, int? basicDocumentId, CancellationToken cancellationToken)
        {
            IQueryable<PrintTemplate> query = _context.PrintTemplates.AsQueryable();

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            }
            else if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RuoRegId == _userInfo.RegionID.Value);
            }
            else
            {
                query = query.Where(x => 1 == 2);
            }

            if (basicDocumentId.HasValue)
            {
                query = query.Where(x => x.BasicDocumentId == basicDocumentId.Value);
            }

            if (searchStr.IsNullOrWhiteSpace())
            {
                searchStr = null;
            }
            else
            {
                searchStr = searchStr.Trim();
            }

            query = query.Where(x => searchStr == null || x.Name.Contains(searchStr));

            var items = await query.Select(x => new PrintFormDropdownViewModel
            {
                Value = $"{x.Id}.trdp",
                Text = $"{x.Name} ({x.BasicDocumentPrintForm.Edition})"
            }).ToListAsync(cancellationToken);

            var printForms = await _context.BasicDocumentPrintForms.Where(i => i.BasicDocumentId == basicDocumentId && i.ReportFormPath != null).OrderByDescending(i => i.Edition)
                .Select(
                i => new PrintFormDropdownViewModel()
                {
                    Value = i.ReportFormPath,
                    Text = $"По подразбиране ({i.Edition})",
                    PrintFormId = i.Id
                })
                .ToListAsync();
            items.InsertRange(0, printForms);

            return items;
        }
    }
}
