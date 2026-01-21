using Helpdesk.Models;
using Helpdesk.Models.Grid;
using Helpdesk.Models.Issue;
using System.Threading.Tasks;

namespace Helpdesk.Services.Interfaces
{
    public interface IIssueService
    {
        Task<IPagedList<IssueViewModel>> GetListAsync(IssuePageListInput input);
        Task<IssueViewModel> GetById(int id);
        Task<int> Create(IssueModel model);
        Task Update(IssueModel model);
        Task Resolve(IssueCommentModel model);
        Task Comment(IssueCommentModel model);
        Task Reopen(IssueReopenModel model);
        Task AssignToMyself(IssueAssignmentModel model);
        Task AssignTo(IssueAssignmentModel model);
        Task LogReadActivity(int? id);
    }
}
