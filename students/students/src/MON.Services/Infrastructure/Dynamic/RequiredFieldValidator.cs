using MON.Models.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Infrastructure.Dynamic
{
    [ExcludeFromCodeCoverage]
    public class RequiredFieldValidator : ValidatorBase
    {
        public RequiredFieldValidator(DynamicEntityItem constraints)
            : base(constraints)
        {

        }

        public override List<KeyValuePair<string, string>> HandleValidation(object value)
        {
            if (Constraints != null)
            {
                if (Constraints.Required)
                {
                    if (value == null)
                    {
                        Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(RequiredFieldValidator)}. Field is required. Value is null"));
                    }
                    else
                    {
                        switch (FieldType)
                        {
                            case Models.Enums.DynamicFieldTypeEnum.TextField:
                                if (string.IsNullOrEmpty(value.ToString()))
                                {
                                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(RequiredFieldValidator)}. Field is required. Value is empty"));
                                }
                                break;
                            case Models.Enums.DynamicFieldTypeEnum.NumberField:
                                if (!decimal.TryParse(value.ToString(), out decimal decVal))
                                {
                                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(RequiredFieldValidator)}. Field is required. Invalid number"));
                                }
                                break;
                            case Models.Enums.DynamicFieldTypeEnum.BooleanField:
                                if (!bool.TryParse(value.ToString(), out bool boolVal))
                                {
                                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(RequiredFieldValidator)}. Field is required. Invalid boolean value"));
                                }
                                break;
                            case Models.Enums.DynamicFieldTypeEnum.DateField:
                                if (!DateTime.TryParse(value.ToString(), out DateTime dateVal))
                                {
                                    Errors.Add(new KeyValuePair<string, string>(Constraints.Id, $"{nameof(RequiredFieldValidator)}. Field is required. Invalid date value"));
                                }
                                break;
                            case Models.Enums.DynamicFieldTypeEnum.AutoComplete:
                            case Models.Enums.DynamicFieldTypeEnum.LabelField:
                            case Models.Enums.DynamicFieldTypeEnum.PersonUniqueIdField:
                            case Models.Enums.DynamicFieldTypeEnum.SchoolYearField:
                            case Models.Enums.DynamicFieldTypeEnum.SelectList:
                            case Models.Enums.DynamicFieldTypeEnum.YearPicker:
                            case Models.Enums.DynamicFieldTypeEnum.YearPickerCombo:
                            default:
                                break;
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
