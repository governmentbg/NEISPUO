using Newtonsoft.Json;

namespace MON.DataAccess
{
    public partial class VStudentLodCurrentAsssessment
    {
        public static explicit operator VStudentLodAsssessment(VStudentLodCurrentAsssessment obj)
        {
            return JsonConvert.DeserializeObject<VStudentLodAsssessment>(JsonConvert.SerializeObject(obj));
        }
    }
}