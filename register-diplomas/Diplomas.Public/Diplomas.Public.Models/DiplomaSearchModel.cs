namespace Diplomas.Public.Models
{
    public class DiplomaSearchModel
    {
        public int? IdType { get; set; }
        public string IdNumber { get; set; }
        public string DocNumber { get; set; }
        public string ReCaptchaToken { get; set; }
    }
}
