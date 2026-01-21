using MON.Models.Dynamic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Infrastructure.Dynamic
{
    [ExcludeFromCodeCoverage]
    public class TextMaxLengthFieldValidator : ValidatorBase
    {
        public TextMaxLengthFieldValidator(DynamicEntityItem constraints)
            : base(constraints)
        {

        }

        public override List<KeyValuePair<string, string>> HandleValidation(object value)
        {
            if (Constraints != null && value != null && Constraints.Max.HasValue)
            {
                string strVal = value.ToString();
                if (strVal.Length > Constraints.Max.Value)
                {
                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Length {strVal.Length} greater than Max: {Constraints.Max.Value}"));
                }
            }

            if (Successor != null)
            {
                Errors.AddRange(Successor.HandleValidation(value));
            }

            return Errors;
        }
    }
}
