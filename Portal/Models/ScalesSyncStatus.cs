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
        [ForeignKey("Scale_ID")]
        public Scale Scale { get; set; }
        public DateTime LastSyncTime { get; set; }
    }
}