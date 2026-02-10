using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class LodModel
    {
        public PersonalDevelopmentSupportModel PersonalDevelopmentSupport { get; set; }
        public IEnumerable<PersonalDevelopmentSupportModel> PersonalDevelopmentSupports { get; set; }

        public IEnumerable<PersonalDevelopmentSupportModel> PersonalDevelopmentSupportsPreviousYears { get; set; }
        public IEnumerable<PersonalDevelopmentSupportModel> PersonalDevelopmentSupportsPreviousYearsGroup { get; set; }

        //feature of newtosoft used to ignore field being serialized.
        public bool ShouldSerializePersonalDevelopmentSupports()
        {
            return false;
        }
    }
}
