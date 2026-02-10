namespace Kontrax.Regix.Core.RegixModels.AZ.JobSeekerStatus
{
    public partial class JobSeekerStatusResponse
    {
        public override string ToString()
        {
            var seekerData = this.JobSeekerPersonData;
            return $"{seekerData.RegistrationDate.ToShortDateString()} {seekerData.RegistrationStatus} {seekerData.RegistrationDBT}";
        }
    }
}
