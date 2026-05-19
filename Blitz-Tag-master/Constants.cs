namespace Blitz_Tag
{
    public static class Constants
    {
        public const string TitleId = "A4A8D";
        public const string SecretKey = "ECWGM54C96A1RKHX5IPXPXTMSSS4QG8GJMYEKWE1HG7W56WPXP";
        public const string AppSecret = "OC|26914022181555603|b8f542e0c27acae5bb4b2a95a9609f80\n";
        public const string AppId = "26914022181555603";
        public static readonly string[] AllowedUnityVersions = [
            "6000.2.9f1",
            "2022.3.2f1",
            "2019.3.15f1"
        ];
        public static readonly string[] AllowedUserAgents = [
            "UnityPlayer/6000.2.9f1 (UnityWebRequest/1.0, libcurl/8.10.1-DEV)",
            "UnityPlayer/2022.3.2f1 (UnityWebRequest/1.0, libcurl/7.84.0-DEV)",
            "UnityPlayer/2019.3.15f1 (UnityWebRequest/1.0, libcurl/7.52.0-DEV)"
        ];
        
        public const string AllowedSharedBlocksCharacters = "CFGHKMNPRTWXZ256789";

        // Event Codes
        public class Events
        {
            public const byte CosmeticPurchase = 9;

            public const byte ReportPlayer = 50;

            public const byte ReportMute = 51;
        }
    }
}
