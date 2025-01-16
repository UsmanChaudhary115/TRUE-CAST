using System.ComponentModel.DataAnnotations;

namespace OVS.Models
{
    public class ElectionResult
    {
        [Key]
        public int ElectionResultId { get; set; } 
        public string ElectionTitle { get; set; }
        public DateTime ElectionStartDate { get; set; }
        public DateTime ElectionEndDate { get; set; } 
        public string CandidateName { get; set; }
        public string CandidatePartyAffiliation { get; set; } 
        public int VotesReceived { get; set; }
        public int TotalVotes { get; set; }
    }
}
