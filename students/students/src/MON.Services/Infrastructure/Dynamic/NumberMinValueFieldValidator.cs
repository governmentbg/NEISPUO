using MON.Models.Dynamic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Infrastructure.Dynamic
{
    [ExcludeFromCodeCoverage]
    public class NumberMinValueFieldValidator : ValidatorBase
    {
        public NumberMinValueFieldValidator(DynamicEntityItem constraints)
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
                    if (!decimal.TryParse(value.ToString(), out decimal decVal))
                    {
                        Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Invalid number. Min: {Constraints.Min.Value}"));
                    }
                    else
                    {
                        if (decVal < Constraints.Min.Value)
                        {
                            Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Less number {decVal} than min value {Constraints.Min.Value}"));
                        }
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
