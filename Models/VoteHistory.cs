namespace OVS.Models
{
    public class VoteHistory
    {
        public int VoteHistoryID { get; set; }
        public string UserID { get; set; }
        public int ElectionID { get; set; }
        public string ElectionTitle { get; set; }
        public int CandidateID { get; set; }
        public string CandidateName { get; set; }
        public DateTime VoteDateTime { get; set; }
        public string Status { get; set; } // Valid, Invalid, etc.
    }
}
