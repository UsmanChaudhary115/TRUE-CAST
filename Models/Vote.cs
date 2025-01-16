using System.ComponentModel.DataAnnotations;

namespace OVS.Models
{
    public class Vote
    {
        [Key]
        public int VoteID { get; set; }
        public int VoterID { get; set; }
        public int ElectionID { get; set; }
        public int CandidateID { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
