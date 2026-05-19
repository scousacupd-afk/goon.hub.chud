namespace Blitz_Tag.Models
{
    public class IapMothershipSubscription
    {
        public string SubscriptionId { get; set; } = "";

        public DateTimeOffset EarliestStartDate { get; set; }

        public DateTimeOffset CurrentStartDate { get; set; }

        public DateTimeOffset MostRecentBillingCycleStartDate { get; set; }

        public DateTimeOffset MostRecentBillingCycleEndDate { get; set; }

        public int TotalLifetimeSeconds { get; set; }

        public bool IsActive { get; set; }

        public bool IsCancelling { get; set; }

        public string Sku { get; set; } = "";

        public string PlayerId { get; set; } = "";

        public string TrialType { get; set; } = "";

        public string ExternalServiceName { get; set; } = "";

        public string ExternalSubscriptionId { get; set; } = "";

        public string SubscriptionCatalogItemId { get; set; } = "";
    }
}