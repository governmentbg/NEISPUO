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
    using Helpdesk.Models.Configuration;
    using Microsoft.Extensions.Options;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;

    public class QuestionService : BaseService, IQuestionService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;


        public QuestionService(HelpdeskContext context,
            IBlobService blobService, IOptions<BlobServiceConfig> blobServiceConfig,
            IUserInfo userInfo,
            ILogger<QuestionService> logger
        )
            : base(context, userInfo, logger)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<int> Create(QuestionModel model)
        {

            var question = new Faquestion()
            {
                Question = model.Question,
                Answer = model.Answer
            };

            _context.Faquestions.Add(question);
            await SaveAsync();

            return question.Id;
        }

        public async Task Update(QuestionModel model)
        {
            Faquestion question = await _context.Faquestions
                            .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (question == null)
            {
                throw new ArgumentNullException(nameof(question), nameof(Faquestion));
            }

            question.Question = model.Question;
            question.Answer = model.Answer;

            await SaveAsync();
        }

        public async Task<QuestionViewModel> GetById(int id)
        {
            var question = await (
                from q in _context.Faquestions
                where q.Id == id
                select new QuestionViewModel()
                {
                    Id = q.Id,
                    Question = q.Question,
                    Answer = q.Answer,
                    CreateDate = q.CreateDate,
                    ModifyDate = q.ModifyDate
                }).FirstOrDefaultAsync();

            string pattern = "\\(/file/(\\d+)\\)";
            if (question != null)
            {
                question.Answer = Regex.Replace(question.Answer, pattern, m =>
                {
                    DocumentViewModel viewModel = new DocumentViewModel
                    {
                        BlobId = Convert.ToInt32(m.Groups[1].Value)
                    };

                    if (viewModel.BlobId.HasValue)
                    {
                        DocumentExtensions.CalcHmac(viewModel, _blobServiceConfig);
                    }
                    return $"({viewModel.BlobServiceUrl}/{viewModel.BlobId}?t={viewModel.UnixTimeSeconds}&h={viewModel.Hmac})";
                });
            }

            return question;
        }

        public async Task<QuestionViewModel> GetEditModelById(int id)
        {
            var question = await (
                from q in _context.Faquestions
                where q.Id == id
                select new QuestionViewModel()
                {
                    Id = q.Id,
                    Question = q.Question,
                    Answer = q.Answer,
                    CreateDate = q.CreateDate,
                    ModifyDate = q.ModifyDate
                }).FirstOrDefaultAsync();

            return question;
        }

        public async Task<IPagedList<QuestionViewModel>> GetListAsync(QuestionPageListInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(PagedListInput));

            IQueryable<Faquestion> query = _context.Faquestions.AsNoTracking();


            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Question.Contains(input.Filter)
                    || predicate.Answer.Contains(input.Filter));

            IQueryable<QuestionViewModel> questions = query
                .Select(i => new QuestionViewModel()
                {
                    Id = i.Id,
                    Question = i.Question,
                    Answer = i.Answer,
                    CreateDate = i.CreateDate,
                    ModifyDate = i.ModifyDate
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await questions.CountAsync();
            List<QuestionViewModel> items = await questions.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }
    }
}
