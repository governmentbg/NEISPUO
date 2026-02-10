namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Dropdown;
    using MON.Models.Institution.PrintTemplate;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class PrintTemplateController : BaseApiController
    {
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IBasicDocumentMarginService _basicDocumentMarginService;
        private readonly IUserInfo _userInfo;


        public PrintTemplateController(IPrintTemplateService printTemplateService, IBasicDocumentMarginService basicDocumentMarginService, IUserInfo userInfo)
        {
            _printTemplateService = printTemplateService;
            _basicDocumentMarginService = basicDocumentMarginService;
            _userInfo = userInfo;
        }

        [HttpGet]
        public Task<List<PrintTemplateViewModel>> List(CancellationToken cancellationToken)
        {
            return _printTemplateService.List(cancellationToken);
        }

        [HttpPost]
        public Task Create(PrintTemplateModel model)
        {
            return _printTemplateService.Create(model);
        }

        [HttpGet]
        public Task<PrintTemplateViewModel> GetPrintTemplate(int id, CancellationToken cancellationToken)
        {
            return _printTemplateService.GetById(id, cancellationToken);
        }

        [HttpDelete]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _printTemplateService.Delete(id, cancellationToken);
        }

        [HttpPut]
        public async Task Update(PrintTemplateModel model)
        {
            await _printTemplateService.Update(model);
        }

        [HttpGet]
        public async Task<List<PrintFormDropdownViewModel>> DropdownOptions(string searchStr, int? basicDocumentId, CancellationToken cancellationToken)
        {
            return await _printTemplateService.GetDropdownOptions(searchStr, basicDocumentId, cancellationToken);
        }

        [HttpPut]
        public async Task SetDefaultMargins(BasicDocumentMarginModel model)
        {
            if (!_userInfo.InstitutionID.HasValue && !_userInfo.RegionID.HasValue)
            {
                return;
            }

            Regex regex = new Regex(@"^(\d+).trdp");
            Match match = regex.Match(model.ReportForm);
            if (match.Success)
            {
                // Подадено е име на отчет, което прилича на число, затова го изтегляме от базата
                int id = Convert.ToInt32(match.Groups[1].Value);
                var printTemplate = await _printTemplateService.GetById(id, CancellationToken.None);
                printTemplate.Left1Margin = model.Left1Margin;
                printTemplate.Top1Margin = model.Top1Margin;
                printTemplate.Left2Margin = model.Left2Margin;
                printTemplate.Top2Margin = model.Top2Margin;

                await _printTemplateService.Update(printTemplate);
            }
            else
            {
                model.InstitutionId = _userInfo.InstitutionID;
                model.RuoRegId = _userInfo.RegionID;
                await _basicDocumentMarginService.AddOrUpdate(model);
            }
        }

        [HttpGet]
        public async Task<BasicDocumentMarginModel> GetDefaultMargins(int basicDocumentId, string reportForm, CancellationToken cancellationToken)
        {
            return await _basicDocumentMarginService.Get(_userInfo.InstitutionID, _userInfo.RegionID, basicDocumentId, reportForm, cancellationToken);
        }
    }
}
