using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("ScalesSyncStatuses")]
    public class ScalesSyncStatus
    {
        public int Id { get; set; }
        [Required]
        public long Scale_ID { get; set; }
        public Scale Scale { get; set; }
        public DateTime LastSyncTime { get; set; }
    }
}