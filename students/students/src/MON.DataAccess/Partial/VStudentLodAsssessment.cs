namespace MON.DataAccess
{
    using Newtonsoft.Json;

    public partial class VStudentLodAsssessment
    {
        public static explicit operator VStudentLodCurrentAsssessment(VStudentLodAsssessment obj)
        {
            return JsonConvert.DeserializeObject<VStudentLodCurrentAsssessment>(JsonConvert.SerializeObject(obj));
        }
    }
}