namespace MON.API.Controllers.DocManagement
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Implementations;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;
    using MON.Models.DocManagement;
    using System.Threading;
    using System.Collections.Generic;
    using MON.Models.Grid;
    using MON.DataAccess;

    public class DocManagementApplicationController : BaseApiController
    {
        private readonly DocManagementApplicationService _service;

        public DocManagementApplicationController(DocManagementApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<DocManagementApplicationViewModel>> List([FromQuery] DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<VDocManagementReport>> ReportList([FromQuery] DocManagementReportListInput input, CancellationToken cancellationToken)
        {
            return await _service.ReportList(input, cancellationToken);
        }

        [HttpGet]
        public async Task<DocManagementApplicationViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetBasicDocuments(CancellationToken cancellationToken)
        {
            return await _service.GetBasicDocuments(cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<DocManagementApplicationStatusViewModel>> Statuses(int id, CancellationToken cancellationToken)
        {
            return await _service.GetStatuses(id, cancellationToken);
        }

        [HttpPost]
        public async Task Create(DocManagementApplicationModel model)
        {
            await _service.Create(model);
        }


        [HttpPut]
        public async Task Update(DocManagementApplicationModel model)
        {
            await _service.Update(model);
        }

        [HttpPut]
        public async Task AttachApplicationReport(DocManagementApplicationModel model)
        {
            await _service.AttachApplicationReport(model);
        }

        [HttpPut]
        public async Task ReportDelivery(DocManagementApplicationModel model)
        {
            await _service.ReportDelivery(model);
        }

        [HttpPut]
        public async Task Submit(DocManagementApplicationReturnForCorectionModel model, CancellationToken cancellationToken)
        {
            await _service.Submit(model, cancellationToken);
        }

        [HttpPost]
        public async Task Approve(DocManagementExchangeRequestApproveModel model, CancellationToken cancellationToken)
        {
            await _service.Approve(model, cancellationToken);
        }

        [HttpPost]
        public async Task Reject(DocManagementExchangeRequestRejectModel model, CancellationToken cancellationToken)
        {
            await _service.Reject(model, cancellationToken);
        }

        [HttpPut]
        public async Task ReturnForCorection(DocManagementApplicationReturnForCorectionModel model, CancellationToken cancellationToken)
        {
            await _service.ReturnForCorection(model, cancellationToken);
        }

        [HttpPut]
        public async Task ActionResponse(DocManagementApplicationResponseModel model, CancellationToken cancellationToken)
        {
            await _service.ActionResponse(model, cancellationToken);
        }

        [HttpDelete]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _service.Delete(id, cancellationToken);
        }

        [HttpPost]
        public async Task<FileContentResult> GenerateApplicationReport(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            (string fileName, byte[] fileContents, string contentType) = await _service.GenerateApplicationReport(model, cancellationToken);
            return File(
                fileContents: fileContents,
                contentType: contentType,
                fileDownloadName: fileName
            );
        }

        /// <summary>
        /// Приложение № 6 към чл. 52, ал. 1
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileContentResult> GenerateReport(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            (string fileName, byte[] fileContents, string contentType) = await _service.GenerateReport(model, cancellationToken);
            return File(
                fileContents: fileContents,
                contentType: contentType,
                fileDownloadName: fileName
            );
        }

        [HttpPost]
        public async Task<FileContentResult> GenerateDestructionProtocol(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            (string fileName, byte[] fileContents, string contentType) = await _service.GenerateDestructionProtocol(model, cancellationToken);
            return File(
                fileContents: fileContents,
                contentType: contentType,
                fileDownloadName: fileName
            );
        }

        [HttpGet]
        public async Task<IPagedList<VDocManagementDashboard>> Dashboard([FromQuery]DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            return await _service.Dashboard(input, cancellationToken);
        }
    }
}
