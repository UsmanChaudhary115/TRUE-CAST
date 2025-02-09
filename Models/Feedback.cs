using System.ComponentModel.DataAnnotations;

namespace OVS.Models
{
    public class FeedbackModel
    {
        [Key]
        public int FeedbackID { get; set; }
        public string? Name { get; set; }
        public string? Feedback { get; set; }
    }
}
