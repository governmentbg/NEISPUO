namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Question;
    using Helpdesk.Services.Extensions;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq.Dynamic.Core;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Helpdesk.Models.Statistics;

    public class StatisticsService : BaseService, IStatisticsService
    {
        public StatisticsService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<StatisticsService> logger
        )
            : base(context, userInfo, logger)
        {

        }

        public async Task<IPagedList<CategoryStatModel>> GetCategoryListAsync(CategoryStatPageListInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(PagedListInput));

            IQueryable<VCategoryStat> query = _context.VCategoryStats.AsNoTracking();


            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Name.Contains(input.Filter)
                    );

            IQueryable<CategoryStatModel> categories = query
                .Select(i => new CategoryStatModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    AllIssues = i.AllIssues,
                    NewIssues = i.New,
                    AssignedIssues = i.Assigned,
                    ResolvedIssues = i.Resolved,
                    Oldest = i.Oldest,
                    Code = i.Code
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id asc" : input.SortBy);

            int totalCount = await categories.CountAsync();
            List<CategoryStatModel> items = await categories.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

    }
}
