using System.ComponentModel.DataAnnotations;

namespace OVS.Models
    {
        public class Election
        {
            [Key]
            public int ElectionId { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool IsLive { get; set; }
        }

    }
