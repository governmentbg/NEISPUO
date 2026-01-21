using System;

namespace Kontrax.RegiX.Core.TestStandard.Models.Xsd
{
    public class ResponseModel
    {
        public DateTime? DateTime { get; set; }

        public byte[] DocxBytes { get; set; }

        /// <summary>
        /// Ключ за търсене на резултата в кеша при експорт или рендиране на екрана.
        /// </summary>
        public string Guid { get; set; }

        #region Debug информация

        public string BeautifiedResponse { get; set; }

        public string ErrorMessage { get; set; }

        #endregion
    }
}
