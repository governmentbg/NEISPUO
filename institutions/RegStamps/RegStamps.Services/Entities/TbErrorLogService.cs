namespace RegStamps.Services.Entities
{
    using Data.Entities;

    using System.Threading.Tasks;

    public class TbErrorLogService : ITbErrorLogService
    {
        private readonly DataStampsContext context;

        public TbErrorLogService(DataStampsContext context)
            => this.context = context;

        public async Task<int> CreateLogAsync(int schoolId, string url, string actionName, string controllerName, Exception exception)
        {
            this.context.TbErrorLogs.Add(new TbErrorLog 
            { 
                ActionName = actionName,
                ControllerName = controllerName,
                Url = url,
                SchoolId = schoolId,
                ErrorMessage = exception.Message,
                ErrorInnerMessage = exception.InnerException?.Message,
                ErrorStackTrace = exception.StackTrace,
                TimeStamp = DateTime.Now,
            });

            return await this.context.SaveChangesAsync();
        }
    }
}
