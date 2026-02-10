namespace MON.Services.Infrastructure.Dynamic
{
    using MON.Models.Dynamic;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class NumberMaxValueFieldValidator : ValidatorBase
    {
        public NumberMaxValueFieldValidator(DynamicEntityItem constraints)
            : base(constraints)
        {

        }

        public override List<KeyValuePair<string, string>> HandleValidation(object value)
        {
            if (Constraints != null && value != null && Constraints.Max.HasValue)
            {
                if (!decimal.TryParse(value.ToString(), out decimal decVal))
                {
                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Invalid number. Max: {Constraints.Max.Value}"));
                }
                else
                {
                    if (decVal > Constraints.Max.Value)
                    {
                        Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(TextMaxLengthFieldValidator)}. Greater number {decVal} than max value {Constraints.Max.Value}"));
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
