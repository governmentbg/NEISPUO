namespace Kontrax.RegiX.Core.RegiXModels.AV
{
    using System.Collections.Generic;
    using System.Linq;

    public class CompanyDetailsModel
    {
        public string Uic { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string Settlement { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }

        public static CompanyDetailsModel From(Regix.Core.RegixModels.AV.ActualResponseType.ActualStateResponse state)
        {
            if (state == null)
            {
                return null;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(state.Seat?.Address?.Street))
            {
                address.Add($"{state.Seat?.Address?.Street} {state.Seat?.Address?.StreetNumber}");
            }

            if (!string.IsNullOrWhiteSpace(state.Seat?.Address?.Block))
            {
                address.Add($"бл. {state.Seat.Address.Block}");
            }

            if (!string.IsNullOrWhiteSpace(state.Seat?.Address?.Entrance))
            {
                address.Add($"вх. {state.Seat.Address.Entrance}");
            }

            if (!string.IsNullOrWhiteSpace(state.Seat?.Address?.Floor))
            {
                address.Add($"ет. {state.Seat.Address.Floor}");
            }

            if (!string.IsNullOrWhiteSpace(state.Seat?.Address?.Apartment))
            {
                address.Add($"ап. {state.Seat.Address.Apartment}");
            }

            return new CompanyDetailsModel
            {
                Uic = state.UIC,
                Name = $"{state.Company} {state.LegalForm?.LegalFormAbbr}",
                Email = state.Seat?.Contacts?.EMail,
                Phone = state.Seat?.Contacts?.Phone,
                Country = state.Seat?.Address?.Country,
                District = state.Seat?.Address?.District,
                Municipality = state.Seat?.Address?.Municipality,
                Settlement = state.Seat?.Address?.Settlement,
                Area = state.Seat?.Address?.Area,
                Address = string.Join(",", address),
            };
        }

        public static CompanyDetailsModel From(Regix.Core.RegixModels.AV.StateOfPlay.StateOfPlay state)
        {
            if (state == null)
            {
                return null;
            }

            List<string> address = new List<string>();
            var first = state.Subject?.Addresses?.OrderBy(x => x.AddressType.Code).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first?.Street))
            {
                address.Add($"{first.Street} {first?.StreetNumber}");
            }

            if (!string.IsNullOrWhiteSpace(first?.Building))
            {
                address.Add($"бл. {first.Building}");
            }

            if (!string.IsNullOrWhiteSpace(first?.Entrance))
            {
                address.Add($"вх. {first.Entrance}");
            }

            if (!string.IsNullOrWhiteSpace(first?.Floor))
            {
                address.Add($"ет. {first.Floor}");
            }

            if (!string.IsNullOrWhiteSpace(first?.Apartment))
            {
                address.Add($"ап. {first.Apartment}");
            }

            // Не е ясно как да се вземат Country, District, Municipality, Settlement, LocalArea.
            // В адресите има обект Country със свойство Code, примерно 100, което явно е някаква номенклатура.
            // Има и обект Location със свойство Code, примерно 67338, което може би е EKKATE код.
            return new CompanyDetailsModel
            {
                Uic = state.Subject?.UIC?.UIC1,
                Name = state.Subject?.LegalEntitySubject?.CyrillicFullName,
                Email = state.Subject?.Communications?.FirstOrDefault(x => x.Type?.Code == "728")?.Value,
                Phone = state.Subject?.Communications?.FirstOrDefault(x => x.Type?.Code == "721")?.Value,
                Address = string.Join(",", address),
            };
        }
    }
}
