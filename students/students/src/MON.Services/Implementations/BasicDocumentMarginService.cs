namespace MON.Services.Implementations
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Services.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using MON.Shared.ErrorHandling;

    public class BasicDocumentMarginService : BaseService<BasicDocumentMarginService>, IBasicDocumentMarginService
    {
        public BasicDocumentMarginService(DbServiceDependencies<BasicDocumentMarginService> dependencies)
            : base(dependencies)
        {
        }

        public async Task<int> AddOrUpdate(BasicDocumentMarginModel model)
        {
            IQueryable<BasicDocumentMargin> query = _context.BasicDocumentMargins
                .Where(x => x.BasicDocumentId == model.BasicDocumentId
                    && x.BasicDocumentPrintForm.ReportFormPath == model.ReportForm);

            if (model.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == model.InstitutionId.Value);
            }
            else if (model.RuoRegId.HasValue)
            {
                query = query.Where(x => x.RuoRegId == model.RuoRegId);
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            BasicDocumentMargin dbMargin = await query.FirstOrDefaultAsync();
            if (dbMargin == null)
            {
                dbMargin = new BasicDocumentMargin();
                _context.BasicDocumentMargins.Add(dbMargin);
            }

            var basicDocumentPrintForm = _context.BasicDocumentPrintForms.FirstOrDefault(i => i.ReportFormPath == model.ReportForm && i.BasicDocumentId == model.BasicDocumentId);

            dbMargin.InstitutionId = model.InstitutionId;
            dbMargin.RuoRegId = model.RuoRegId;
            dbMargin.Left1Margin = model.Left1Margin;
            dbMargin.Top1Margin = model.Top1Margin;
            dbMargin.Left2Margin = model.Left2Margin;
            dbMargin.Top2Margin = model.Top2Margin;
            dbMargin.BasicDocumentId = model.BasicDocumentId;
            dbMargin.BasicDocumentPrintFormId = basicDocumentPrintForm.Id;

            await SaveAsync();
            return dbMargin.Id;
        }

        public async Task<BasicDocumentMarginModel> Get(int? institutionId, int? regionId, int basicDocumentId, string reportForm, CancellationToken cancellationToken)
        {
            IQueryable<BasicDocumentMargin> query = _context.BasicDocumentMargins
                .Where(x => x.BasicDocumentId == basicDocumentId && x.BasicDocumentPrintForm.ReportFormPath.Equals(reportForm));

            if (institutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == institutionId.Value);
            }
            else if (regionId.HasValue)
            {
                query = query.Where(x => x.RuoRegId == regionId);
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await query.Select(x => new BasicDocumentMarginModel()
            {
                InstitutionId = x.InstitutionId,
                RuoRegId = x.RuoRegId,
                Left1Margin = x.Left1Margin,
                Top1Margin = x.Top1Margin,
                Left2Margin = x.Left2Margin,
                Top2Margin = x.Top2Margin,
                BasicDocumentId = x.BasicDocumentId
            })
            .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
