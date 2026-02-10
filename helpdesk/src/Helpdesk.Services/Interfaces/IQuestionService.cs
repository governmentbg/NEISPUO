namespace Helpdesk.Services.Interfaces
{
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Question;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IQuestionService
    {
        Task<IPagedList<QuestionViewModel>> GetListAsync(QuestionPageListInput input);
        Task<QuestionViewModel> GetById(int id);
        Task<QuestionViewModel> GetEditModelById(int id);
        Task<int> Create(QuestionModel model);
        Task Update(QuestionModel model);
    }
}
