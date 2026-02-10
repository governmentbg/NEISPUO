using System.Collections.Generic;

namespace MON.Models.StudentModels
{

    public class ResourceSupportModel : BaseResourceSupport
    {
        public string ResourceSupportTypeName { get; set; }
        public List<ResourceSupportSpecialistModel> ResourceSupportSpecialists { get; set; }
    }
}
