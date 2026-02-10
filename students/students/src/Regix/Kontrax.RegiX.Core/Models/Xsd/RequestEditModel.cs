using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontrax.RegiX.Core.TestStandard.Models.Xsd
{
    public class RequestEditModel : RequestBaseModel
    {
        /// <summary>
        /// Id-то засега не се POST-ва. Във view-то се използва единсвено за изтегляне на response xml-а.
        /// По-важната функция на това property e да пренесе id-то от GetBatchForEditAsync() към CallAsync().
        /// </summary>
        public int? Id { get; set; }

        public XsdModel Xsd { get; set; }

        [Display(Name = "Помощна информация")]
        public string Help { get; set; }

        #region Debug информация

        public string RequestXml { get; set; }

        /// <summary>
        /// Грешки от непълна конфигурация на RegiXReport-а + съобщение за празни задължителни полета + грешки от XSD валидация.
        /// </summary>
        public List<string> ValidationErrors { get; set; }

        #endregion

        public ResponseModel Response { get; set; }
    }
}
