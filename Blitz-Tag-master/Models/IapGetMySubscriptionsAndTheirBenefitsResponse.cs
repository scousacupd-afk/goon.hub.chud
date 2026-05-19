namespace Blitz_Tag.Models
{
    public class IapGetMySubscriptionsAndTheirBenefitsResponse
    {
        public List<IapMothershipSubscription> Subscriptions { get; set; } = [];

        public Dictionary<string, List<object>> PreviouslyGrantedBenefitsBySubscriptionSku { get; set; } = [];

        public Dictionary<string, List<object>> NewlyGrantedBenefitsBySubscriptionSku { get; set; } = [];

        public bool? SharedGroupDataUpdateSucceeded { get; set; } = null;
    }
}
