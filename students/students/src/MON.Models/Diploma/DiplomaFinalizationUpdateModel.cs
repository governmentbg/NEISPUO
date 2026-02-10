namespace MON.Models.Diploma
{
    public class DiplomaFinalizationUpdateModel
    {
        public int DiplomaId { get; set; }
        public int ConfirmedStepNumber { get; set; }
        public string Signature { get; set; }
    }
}
