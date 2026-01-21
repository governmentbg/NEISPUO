using System.Text;

namespace Kontrax.Regix.Core.RegixModels.NRA
{
    public partial class EmploymentContractsResponse
    {
        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var contract in this.EContracts)
            {
                result.Append($"Работодател: {contract.ContractorName} {contract.ContractorBulstat} с местонахождение {contract.EKATTECode} " +
                    $"От: {contract.StartDate.ToShortDateString()} До: {contract.EndDate.ToShortDateString()} на длъжност {contract.ProfessionCode}/{contract.ProfessionName} Причина: {contract.Reason} Заплата: {contract.Remuneration}\r\n");
            }
            return result.ToString();
        }
    }
}
