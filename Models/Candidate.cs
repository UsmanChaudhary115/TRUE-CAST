using System.ComponentModel.DataAnnotations;

namespace OVS.Models
{
    public class Candidate
    {
        [Key] 
        public int CandidateId { get; set; } 
        public string Name { get; set; } 
        public string CNIC { get; set; }
        public string PartyAffiliation { get; set; }
        public string ContactInfo { get; set; }  
        public string Manifesto { get; set; } 
    }
}
