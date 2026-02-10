using System.Collections.Generic;

namespace MON.Models.Configuration
{

    /// <summary>
    /// Зарежда стойности от student.AppSettings таблицата за ключ InstTypeToClassKindEnrollmentLimit
    /// </summary>
    public class InstTypeToClassTypeConfiguration
    {
        private const string DefaultOperator = "OR";

        /// <summary>
        /// Позволени типове и видове класове при запис в основна клас/група.
        /// Използва се при запис през документ за записване.
        /// </summary>
        public AllowedClassTypesConfiguration InitialEnrollment { get; set; }

        /// <summary>
        /// Позволени типове и видове класове при запис в допълнителна клас/група.
        /// Позволява се записването само в една от позволените (IsCurrent = 1).
        /// </summary>
        public AllowedClassTypesConfiguration SingleEnrollment { get; set; }

        /// <summary>
        /// Позволени типове и видове класове при запис в допълнителна клас/група.
        /// Позволява се записването в неограничен брой.
        /// </summary>
        public AllowedClassTypesConfiguration MultipleEntrollment { get; set; }

        public string Operator { get; set; }

        public bool IsValid(int? classKind, int? classType)
        {
            // Връщаме че е валидно ако липсва classKind или classType.
            if (!classKind.HasValue || !classType.HasValue) return true;

            string oper = this.Operator ?? DefaultOperator;

            if (oper.Equals("AND", System.StringComparison.OrdinalIgnoreCase))
            {
                return SingleEnrollment != null && MultipleEntrollment != null
                    && SingleEnrollment.IsValid(classKind, classType)
                    && MultipleEntrollment.IsValid(classKind, classType);
            }
            else
            {
                return (SingleEnrollment != null && SingleEnrollment.IsValid(classKind, classType))
                    || (MultipleEntrollment != null && MultipleEntrollment.IsValid(classKind, classType));
            }
        }
    }

    public class AllowedClassTypesConfiguration
    {
        private const string DefaultOperator = "AND";

        public HashSet<int> AllowedClassKind { get; set; }
        public HashSet<int> AllowedClassType { get; set; }
        public string Operator { get; set; }

        public bool IsValid(int? classKind, int? classType)
        {
            // Връщаме че е валидно ако липсва classKind или classType.
            if (!classKind.HasValue || !classType.HasValue) return true;

            string oper = this.Operator ?? DefaultOperator;

            if (oper.Equals("AND", System.StringComparison.OrdinalIgnoreCase))
            {
                return AllowedClassKind != null && AllowedClassType != null
                    && AllowedClassKind.Contains(classKind ?? default)
                    && AllowedClassType.Contains(classType ?? default);
            }
            else
            {
                return (AllowedClassKind != null && AllowedClassKind.Contains(classKind ?? default))
                    || (AllowedClassType != null && AllowedClassType.Contains(classType ?? default));
            }
        }
    }
}
