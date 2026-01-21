namespace Helpdesk.Services.Interfaces
{
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Question;
    using Helpdesk.Models.Statistics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStatisticsService
    {
        Task<IPagedList<CategoryStatModel>> GetCategoryListAsync(CategoryStatPageListInput input);
    }
}
