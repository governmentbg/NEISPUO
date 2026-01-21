namespace MON.Services.Infrastructure.Dynamic
{
    using MON.Models.Dynamic;
    using MON.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class DynamicFieldValueValidationContext
    {
        private readonly DynamicEntityItem _constraints;
        private readonly object _value;

        protected readonly DynamicFieldTypeEnum _fieldType;

        public DynamicFieldValueValidationContext(object value, DynamicEntityItem constraints)
        {
            _constraints = constraints;
            _value = value;

            if (_constraints != null && Enum.TryParse(_constraints.Type, true, out DynamicFieldTypeEnum fieldType))
            {
                _fieldType = fieldType;
            }
        }

        public List<KeyValuePair<string, string>> Validate()
        {
            ValidatorBase chain = GetValidationChain();
            if (chain == null) return null;

            return chain.HandleValidation(_value);
        }

        private ValidatorBase GetValidationChain()
        {
            switch (_fieldType)
            {
                case DynamicFieldTypeEnum.NumberField:
                    return Chain(new ValidatorBase[] {
                        new RequiredFieldValidator(_constraints),
                        new NumberMinValueFieldValidator(_constraints),
                        new NumberMaxValueFieldValidator(_constraints)
                        });
                case DynamicFieldTypeEnum.TextField:
                    return Chain(new ValidatorBase[] {
                        new RequiredFieldValidator(_constraints),
                        new TextMinLengthFieldValidator(_constraints),
                        new TextMaxLengthFieldValidator(_constraints)
                        });
                case DynamicFieldTypeEnum.AutoComplete:
                case DynamicFieldTypeEnum.BooleanField:
                case DynamicFieldTypeEnum.DateField:
                case DynamicFieldTypeEnum.LabelField:
                case DynamicFieldTypeEnum.PersonUniqueIdField:
                case DynamicFieldTypeEnum.SchoolYearField:
                case DynamicFieldTypeEnum.SelectList:
                case DynamicFieldTypeEnum.YearPicker:
                case DynamicFieldTypeEnum.YearPickerCombo:
                default:
                    return null;
            }
        }

        private ValidatorBase Chain(ValidatorBase[] validatos)
        {
            if (validatos == null || validatos.Length == 0) return null;

            for (int i = 0; i < validatos.Length; i++)
            {
                if (i < validatos.Length - 1)
                {
                    validatos[i].SetSuccessor(validatos[i + 1]);
                }
            }

            return validatos[0];
        }
    }
}
