namespace Blitz_Tag.Models
{
    public class IapGetMySubscriptionsAndTheirBenefitsRequest
    {
        public bool Refresh;

        public bool? SkipBenefitsCheck;

        public bool? SkipSharedGroupDataUpdate;

        public string MothershipId { get; set; } = "";

        public string MothershipToken { get; set; } = "";

        public string MothershipEnvId { get; set; } = "";

        public string MothershipDeploymentId { get; set; } = "";
    }
}
