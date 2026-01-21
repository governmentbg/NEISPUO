using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MON.Models;
using MON.Models.Grid;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MON.API.Controllers
{

    public class BasicDocumentController : BaseApiController
    {
        private readonly IBasicDocumentService _service;
        private readonly IUserInfo _userInfo;

        public BasicDocumentController(IBasicDocumentService service, IUserInfo userInfo)
        {
            _service = service;
            _userInfo = userInfo;
        }

        [HttpGet]
        public async Task<IPagedList<BasicDocumentSequenceViewModel>> GetBasicDocumentSequences([FromQuery] BasicDocumentSequenceListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetBasicDocumentSequencesAsync(input, cancellationToken);
        }

        [HttpDelete]
        public async Task DeleteBasicDocumentSequence([FromQuery] int id)
        {
            await _service.DeleteBasicDocumentSequenceAsync(id);
        }

        [HttpGet]
        public async Task<List<BasicDocumentSequenceViewModel>> GetNextBasicDocumentSequence([FromQuery] int basicDocumentId, int count = 1, DateTime? regDate = null)
        {
            var sequences = new List<BasicDocumentSequenceViewModel>();
            int iterations = 0;
            int errors = 0;
            // Повтаряме до 5 пъти, ако има грешка
            while (iterations < count && errors <=5)
            {
                try
                {
                    var sequence = await _service.GetNextBasicDocumentSequence(basicDocumentId, regDate);
                    if (sequence != null)
                    {
                        sequences.Add(sequence);
                        iterations++;
                    }
                }
                catch (Microsoft.Data.SqlClient.SqlException ex)
                {
                    errors++;
                    if (ex.Number > 5000)
                    {
                        throw new ApiException(ex.Message, ex);
                    }
                }
            }
            return sequences;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForBasicDocumentsListShow)]
        public async Task<IPagedList<BasicDocumentModel>> List([FromQuery] DiplomaTypesListInput input)
        {
            return await _service.List(input);
        }

        [HttpGet]
        //Add policy for permissions
        public async Task<BasicDocumentModel> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForBasicDocumentEdit)]
        public async Task<BasicDocumentTemplateUpdateModel> LoadTemplate(int? id)
        {
            return await _service.GetBasicDocumentTemplate(id);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForBasicDocumentEdit)]
        public async Task SaveTemplate(BasicDocumentTemplateUpdateModel model)
        {
            await _service.SaveBasicDocumentTemplate(model);
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForBasicDocumentEdit)]
        public async Task IncludeInRegister([FromQuery]int id)
        {
            await _service.IncludeInRegister(id);
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForBasicDocumentEdit)]
        public async Task ExcludeFromRegister([FromQuery] int id)
        {
            await _service.ExcludeFromRegister(id);
        }

        [HttpGet]
        public async Task<string> GetSchema(int id)
        {
            return await _service.GetSchema(id);
        }

        [HttpGet]
        public async Task<string> GetSchemaByTemplateId(int templateId)
        {
            return await _service.GetSchemaByTemplateId(templateId);
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> PrintFormDropdownOptions(string searchStr, int? basicDocumentId)
        {
            return await _service.GetPrintFormDropdownOptions(searchStr, basicDocumentId);
        }
    }
}
