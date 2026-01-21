using System.Collections.Generic;
using System.Linq;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class KeyElementViewModel
    {
        public string LocalName { get; set; }

        public IEnumerable<KeyTypeModel> KeyTypes { get; set; }  // ЕГН, ЛНЧ, ЕИК.

        public string TypeNames
        {
            get
            {
                return KeyTypes != null ? string.Join(" или ", KeyTypes.Select(t => t.Name)) : null;
            }
        }
    }
}
