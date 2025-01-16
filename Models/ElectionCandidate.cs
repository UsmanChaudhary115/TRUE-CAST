using System.ComponentModel.DataAnnotations;

namespace OVS.Models
{
    public class ElectionCandidate
    {
        [Key]
        public int ElectionCandidateId { get; set; }
        public int ElectionId { get; set; } 
        public int CandidateId { get; set; }
        public int VoteCount { get; set; } 
        public bool isWinning { get; set; }

    }
}