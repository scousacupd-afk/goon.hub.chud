namespace Blitz_Tag.Models
{
    public class PhotonAuthRequestData
    {
        public string? AppId { get; set; }
        
        public string? AppVersion { get; set; }
        
        public string? Ticket { get; set; }
        
        public string? Nonce { get; set; }
        
        public string? MothershipEnvId { get; set; }
        
        public string? MothershipDeploymentId { get; set; }
        
        public string? MothershipToken { get; set; }
        
        public string? Platform { get; set; }
        
        public string? Zone { get; set; }
        
        public string? SubZone { get; set; }
        
        public bool? IsPublic { get; set; }
    }
}
