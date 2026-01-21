namespace MonProjects.Api.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Swashbuckle.AspNetCore.Annotations;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DatasController : BaseController
    {
        private readonly IMsSqlNeispuoService msSqlNeispuoService;

        public DatasController(IMsSqlNeispuoService msSqlNeispuoService)
            => this.msSqlNeispuoService = msSqlNeispuoService;
        

        public int ExtSystemId => this.User.GetCertificateExtSystemId();
        public string Thumbprint => this.User.GetCertificateTrumbprint();
        public string ProcedureName => this.User.GetProcedureName();
        public string ProcedureParameters => this.User.GetProcedureParameters();
        public bool IsProcedureReturnArray => this.User.IsProcedureReturnArray();

        [HttpGet]
        [SwaggerOperation(
            Summary = "Main Logic to Array or Object",
            Description = "Proc ")]
        [ProducesResponseType(typeof(IEnumerable<dynamic>), 200)]
        public async Task<IActionResult> GetDatasAsync()
            => Ok(this.IsProcedureReturnArray
                ? await this.ExecuteProcedureToArrayAsync()
                : await this.ExecuteProcedureToObjectAsync());

        private async Task<IEnumerable<dynamic>> ExecuteProcedureToArrayAsync()
             => await this.msSqlNeispuoService
                            .ExecuteListAsync<dynamic>(this.ProcedureName, base.CheckIsParametersExist(this.ProcedureParameters));

        private async Task<dynamic> ExecuteProcedureToObjectAsync()
             => await this.msSqlNeispuoService
                            .ExecuteFirstAsync<dynamic>(this.ProcedureName, base.CheckIsParametersExist(this.ProcedureParameters));

    }
}
