namespace MON.API.Controllers.DocManagement
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.DocManagement;
    using MON.Models.Grid;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;
    using System.Threading;
    using MON.Services.Implementations;
    using System.Collections.Generic;

    public class DocManagementExchangeController : BaseApiController
    {
        private readonly DocManagementExchangeService _service;

        public DocManagementExchangeController(DocManagementExchangeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<DocManagementFreeDocListModel>> ListFree([FromQuery] DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            return await _service.ListFree(input, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<DocManagementFreeDocExchageListModel>> FreeForExchange([FromQuery] DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetFreeForExchange(input, cancellationToken);
        }

        [HttpPost]
        public async Task CreateRequest(DocManagementExchangeRequestModel model)
        {
            await _service.CreateRequest(model);
        }

        [HttpDelete]
        public async Task DeleteRequest(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteRequest(id, cancellationToken);
        }

        [HttpPost]
        public async Task ApproveRequest(DocManagementExchangeRequestApproveModel model, CancellationToken cancellationToken)
        {
            await _service.ApproveRequest(model, cancellationToken);
        }

        [HttpPost]
        public async Task RejectRequest(DocManagementExchangeRequestRejectModel model, CancellationToken cancellationToken)
        {
            await _service.RejectRequest(model, cancellationToken);
        }

        [HttpPost]
        public async Task<FileContentResult> GenerateProtocol(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            (string fileName, byte[] fileContents, string contentType) = await _service.GenerateProtocol(model, cancellationToken);
            return File(
                fileContents: fileContents,
                contentType: contentType,
                fileDownloadName: fileName
            );
        }
    }
}
