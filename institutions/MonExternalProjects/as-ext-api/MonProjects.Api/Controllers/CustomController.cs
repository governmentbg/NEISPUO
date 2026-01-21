namespace MonProjects.Api.Controllers
{
    using Constants;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Swashbuckle.AspNetCore.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CustomController : BaseController
    {
        private readonly IMsSqlNeispuoService msSqlNeispuoService;

        public CustomController(IMsSqlNeispuoService msSqlNeispuoService)
            => this.msSqlNeispuoService = msSqlNeispuoService;

        public string ProcedureName => this.User.GetProcedureName();
        public string ProcedureParameters => this.User.GetProcedureParameters();

        [HttpGet]
        [SwaggerOperation(
            Summary = "Custom Logics to Object",
            Description = "Proc")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetCustomDatasAsync()
            => Ok(this.ProcedureName.Contains(AppConstants.PipeString)
                  ? await this.ProcedureResultDataToObject(base.CheckIsParametersExist(this.ProcedureParameters))
                  : throw new Exception(AppExceptions.NoValidStructureOfModelDataExceptionMessage));

        private async Task<object> ProcedureResultDataToObject(object parameters)
        {
            IDictionary<string, object> model = new ExpandoObject();

            IEnumerable<string> procedureResultsParts = this.ProcedureName
                                                    .Split(new char[] { AppConstants.PipeChar }, StringSplitOptions.RemoveEmptyEntries);

            if (!procedureResultsParts.All(x => x.Contains(AppConstants.ColonString)))
            {
                throw new Exception(AppExceptions.NoValidStructureOfModelParametersExceptionMessage);
            }

            foreach (var procedureResultPart in procedureResultsParts)
            {
                IEnumerable<string> procedureParts = procedureResultPart
                                                    .Split(new char[] { AppConstants.ColonChar }, StringSplitOptions.RemoveEmptyEntries);

                if (procedureParts.Count() != 2)
                {
                    model.Clear();
                    break;
                }

                // first is model object name, second is procedure result
                model.Add(procedureParts.FirstOrDefault(), await this.msSqlNeispuoService.ExecuteListAsync<object>(procedureParts.LastOrDefault(), parameters));
            }

            return model;
        }
    }
}
