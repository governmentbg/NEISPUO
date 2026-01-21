using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Neispuo.Tools.Services.Interfaces
{
    public interface IStatsService
    {
        /// <summary>
        /// Генериране на общи справки за период
        /// </summary>
        /// <param name="period">Период в минутир</param>
        /// <returns></returns>
        Task GenerateCommonStatsAsync(int period);
    }
}
