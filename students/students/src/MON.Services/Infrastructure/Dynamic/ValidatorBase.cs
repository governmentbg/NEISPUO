using MON.Models.Dynamic;
using MON.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Infrastructure.Dynamic
{
    [ExcludeFromCodeCoverage]
    public abstract class ValidatorBase
    {
        protected ValidatorBase Successor { get; private set; }
        protected List<KeyValuePair<string, string>> Errors { get; private set; }
        protected DynamicFieldTypeEnum FieldType { get; private set; }
        protected DynamicEntityItem Constraints { get; private set; }

        public ValidatorBase(DynamicEntityItem constraints)
        {
            Constraints = constraints;
            Errors = new List<KeyValuePair<string, string>>();
            if (Constraints != null && Enum.TryParse(Constraints.Type, true, out DynamicFieldTypeEnum fieldType))
            {
                FieldType = fieldType;
            }
        }

        public abstract List<KeyValuePair<string, string>> HandleValidation(object value);

        /// <summary>
        /// Добавя във веригата следващия валидатор.
        /// </summary>
        /// <param name="successor"></param>
        public ValidatorBase SetSuccessor(ValidatorBase successor)
        {
            Successor = successor;

            return successor;
        }
    }
}
