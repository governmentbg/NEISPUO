using MON.Models.Dynamic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Infrastructure.Dynamic
{
    [ExcludeFromCodeCoverage]
    public class TextMinLengthFieldValidator : ValidatorBase
    {
        public TextMinLengthFieldValidator(DynamicEntityItem constraints)
            : base(constraints)
        {

        }

        public override List<KeyValuePair<string, string>> HandleValidation(object value)
        {
            if (Constraints != null && Constraints.Min.HasValue)
            {
                if (value == null)
                {
                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Value is null. Min: {Constraints.Min.Value}"));
                }
                else
                {
                    string strVal = value.ToString();
                    if (strVal.Length < Constraints.Min.Value)
                    {
                        Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Length {strVal.Length} less than Min: {Constraints.Min.Value}"));
                    }
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
